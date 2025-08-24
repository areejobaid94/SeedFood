using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.Auditing.Dto;
using Infoseed.MessagingPortal.Auditing.Exporting;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.EntityHistory;
using EntityHistoryHelper = Infoseed.MessagingPortal.EntityHistory.EntityHistoryHelper;
using System;

namespace Infoseed.MessagingPortal.Auditing
{
    [DisableAuditing]
    [AbpAuthorize(AppPermissions.Pages_Administration_AuditLogs)]
    public class AuditLogAppService : MessagingPortalAppServiceBase, IAuditLogAppService
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly IRepository<EntityChange, long> _entityChangeRepository;
        private readonly IRepository<EntityChangeSet, long> _entityChangeSetRepository;
        private readonly IRepository<EntityPropertyChange, long> _entityPropertyChangeRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IAuditLogListExcelExporter _auditLogListExcelExporter;
        private readonly INamespaceStripper _namespaceStripper;
        private readonly IAbpStartupConfiguration _abpStartupConfiguration;

        public AuditLogAppService(
            IRepository<AuditLog, long> auditLogRepository,
            IRepository<User, long> userRepository,
            IAuditLogListExcelExporter auditLogListExcelExporter,
            INamespaceStripper namespaceStripper,
            IRepository<EntityChange, long> entityChangeRepository,
            IRepository<EntityChangeSet, long> entityChangeSetRepository,
            IRepository<EntityPropertyChange, long> entityPropertyChangeRepository,
            IAbpStartupConfiguration abpStartupConfiguration)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
            _auditLogListExcelExporter = auditLogListExcelExporter;
            _namespaceStripper = namespaceStripper;
            _entityChangeRepository = entityChangeRepository;
            _entityChangeSetRepository = entityChangeSetRepository;
            _entityPropertyChangeRepository = entityPropertyChangeRepository;
            _abpStartupConfiguration = abpStartupConfiguration;
        }

        #region audit logs

        public async Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input)
        {
            try
            {
                var query = CreateAuditLogAndUsersQuery(input);

                var resultCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

                var auditLogListDtos = ConvertToAuditLogListDtos(results);

                return new PagedResultDto<AuditLogListDto>(resultCount, auditLogListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input)
        {
            try
            {
                var auditLogs = await CreateAuditLogAndUsersQuery(input)
                .AsNoTracking()
                .OrderByDescending(al => al.AuditLog.ExecutionTime)
                .ToListAsync();

                var auditLogListDtos = ConvertToAuditLogListDtos(auditLogs);

                return _auditLogListExcelExporter.ExportToFile(auditLogListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private List<AuditLogListDto> ConvertToAuditLogListDtos(List<AuditLogAndUser> results)
        {
            try
            {
                return results.Select(
                result =>
                {
                    var auditLogListDto = ObjectMapper.Map<AuditLogListDto>(result.AuditLog);
                    auditLogListDto.UserName = result.User?.UserName;
                    auditLogListDto.ServiceName = _namespaceStripper.StripNameSpace(auditLogListDto.ServiceName);
                    return auditLogListDto;
                }).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private IQueryable<AuditLogAndUser> CreateAuditLogAndUsersQuery(GetAuditLogsInput input)
        {
            try
            {
                var query = from auditLog in _auditLogRepository.GetAll()
                            join user in _userRepository.GetAll() on auditLog.UserId equals user.Id into userJoin
                            from joinedUser in userJoin.DefaultIfEmpty()
                            where auditLog.ExecutionTime >= input.StartDate && auditLog.ExecutionTime <= input.EndDate
                            select new AuditLogAndUser { AuditLog = auditLog, User = joinedUser };

                query = query
                    .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                    .WhereIf(!input.ServiceName.IsNullOrWhiteSpace(), item => item.AuditLog.ServiceName.Contains(input.ServiceName))
                    .WhereIf(!input.MethodName.IsNullOrWhiteSpace(), item => item.AuditLog.MethodName.Contains(input.MethodName))
                    .WhereIf(!input.BrowserInfo.IsNullOrWhiteSpace(), item => item.AuditLog.BrowserInfo.Contains(input.BrowserInfo))
                    .WhereIf(input.MinExecutionDuration.HasValue && input.MinExecutionDuration > 0, item => item.AuditLog.ExecutionDuration >= input.MinExecutionDuration.Value)
                    .WhereIf(input.MaxExecutionDuration.HasValue && input.MaxExecutionDuration < int.MaxValue, item => item.AuditLog.ExecutionDuration <= input.MaxExecutionDuration.Value)
                    .WhereIf(input.HasException == true, item => item.AuditLog.Exception != null && item.AuditLog.Exception != "")
                    .WhereIf(input.HasException == false, item => item.AuditLog.Exception == null || item.AuditLog.Exception == "");
                return query;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        #endregion

        #region entity changes 
        public List<NameValueDto> GetEntityHistoryObjectTypes()
        {
            try
            {
                var entityHistoryObjectTypes = new List<NameValueDto>();
                var enabledEntities = (_abpStartupConfiguration.GetCustomConfig()
                    .FirstOrDefault(x => x.Key == EntityHistoryHelper.EntityHistoryConfigurationName)
                    .Value as EntityHistoryUiSetting)?.EnabledEntities ?? new List<string>();

                if (AbpSession.TenantId == null)
                {
                    enabledEntities = EntityHistoryHelper.HostSideTrackedTypes.Select(t => t.FullName).Intersect(enabledEntities).ToList();
                }
                else
                {
                    enabledEntities = EntityHistoryHelper.TenantSideTrackedTypes.Select(t => t.FullName).Intersect(enabledEntities).ToList();
                }

                foreach (var enabledEntity in enabledEntities)
                {
                    entityHistoryObjectTypes.Add(new NameValueDto(L(enabledEntity), enabledEntity));
                }

                return entityHistoryObjectTypes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<PagedResultDto<EntityChangeListDto>> GetEntityChanges(GetEntityChangeInput input)
        {
            try
            {
                var query = CreateEntityChangesAndUsersQuery(input);

                var resultCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

                var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

                return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<PagedResultDto<EntityChangeListDto>> GetEntityTypeChanges(GetEntityTypeChangeInput input)
        {
            try
            {
                // Fix for: https://github.com/aspnetzero/aspnet-zero-core/issues/2101
                var entityId = "\"" + input.EntityId + "\"";

                var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                            join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange.EntityChangeSetId
                            join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                            where entityChange.EntityTypeFullName == input.EntityTypeFullName &&
                                  (entityChange.EntityId == input.EntityId || entityChange.EntityId == entityId)
                            select new EntityChangeAndUser
                            {
                                EntityChange = entityChange,
                                User = user
                            };

                var resultCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

                var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

                return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<FileDto> GetEntityChangesToExcel(GetEntityChangeInput input)
        {
            try
            {
                var entityChanges = await CreateEntityChangesAndUsersQuery(input)
                .AsNoTracking()
                .OrderByDescending(ec => ec.EntityChange.EntityChangeSetId)
                .ThenByDescending(ec => ec.EntityChange.ChangeTime)
                .ToListAsync();

                var entityChangeListDtos = ConvertToEntityChangeListDtos(entityChanges);

                return _auditLogListExcelExporter.ExportToFile(entityChangeListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<List<EntityPropertyChangeDto>> GetEntityPropertyChanges(long entityChangeId)
        {
            try
            {
                var entityPropertyChanges = (await _entityPropertyChangeRepository.GetAllListAsync())
                .Where(epc => epc.EntityChangeId == entityChangeId);

                return ObjectMapper.Map<List<EntityPropertyChangeDto>>(entityPropertyChanges);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private List<EntityChangeListDto> ConvertToEntityChangeListDtos(List<EntityChangeAndUser> results)
        {
            try
            {
                return results.Select(
                result =>
                {
                    var entityChangeListDto = ObjectMapper.Map<EntityChangeListDto>(result.EntityChange);
                    entityChangeListDto.UserName = result.User?.UserName;
                    return entityChangeListDto;
                }).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private IQueryable<EntityChangeAndUser> CreateEntityChangesAndUsersQuery(GetEntityChangeInput input)
        {
            try
            {
                var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                            join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange.EntityChangeSetId
                            join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                            where entityChange.ChangeTime >= input.StartDate && entityChange.ChangeTime <= input.EndDate
                            select new EntityChangeAndUser
                            {
                                EntityChange = entityChange,
                                User = user
                            };

                query = query
                    .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                    .WhereIf(!input.EntityTypeFullName.IsNullOrWhiteSpace(), item => item.EntityChange.EntityTypeFullName.Contains(input.EntityTypeFullName));

                return query;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        #endregion
    }
}
