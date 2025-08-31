using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Framework.Data;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Evaluations.Dtos;
using Infoseed.MessagingPortal.Evaluations.Exporting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Framework.Data.Sql;
using Infoseed.MessagingPortal.WhatsApp;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Diagnostics;
using Microsoft.Azure.Documents.SystemFunctions;

namespace Infoseed.MessagingPortal.Evaluations
{
    public class EvaluationsAppService : MessagingPortalAppServiceBase, IEvaluationsAppService
	{

        private readonly IEvaluationExcelExporter _evaluationsExcelExporter;
        private readonly IRepository<Evaluation, long> _evaluationRepository;
        private readonly IRepository<Contact> _lookup_customerRepository;
        private readonly string _postgresConnection;
        public EvaluationsAppService(IRepository<Evaluation, long> evaluationRepository, IEvaluationExcelExporter evaluationsExcelExporter, IRepository<Contact> lookup_customerRepository,
            IConfiguration configuration)
		{
			_evaluationRepository = evaluationRepository;

            _lookup_customerRepository=lookup_customerRepository;
            _evaluationsExcelExporter = evaluationsExcelExporter;
            _postgresConnection = configuration.GetConnectionString("postgres");
        }
        public EvaluationsAppService()
        {

        }
        public async Task<PagedResultDto<GetEvaluationForViewDto>> GetAll(GetAllEvaluationsInput input)
        {
            try
            {
                int totalCount = 0;
                int? orderStatus = null;
                string sorting = null;
                if (!string.IsNullOrEmpty(input.Filter))
                {
                    orderStatus= int.Parse(input.Filter);
                }
                if (!string.IsNullOrEmpty(input.Sorting))
                {
                    sorting = input.Sorting;
                }

                List<GetEvaluationForViewDto> itemes = GetALLEvaluation(input.SkipCount, input.MaxResultCount, input.Sorting, out totalCount);
                //itemes= itemes.OrderBy(x=>x.st)
                return new PagedResultDto<GetEvaluationForViewDto>(
                 totalCount,
                 itemes
             );
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<GetEvaluationForViewDto> GetALLEvaluation(int pageOffset, int pageSize, string sorting, out int totalCount)
        {
            try
            {
                List<GetEvaluationForViewDto> lstGetEvaluationViewDto = new List<GetEvaluationForViewDto>();
                totalCount = 0;

                using (var conn = new Npgsql.NpgsqlConnection(_postgresConnection))
                {
                    conn.Open();

                    string query = "SELECT * FROM dbo.evaluation_get(@p_tenant_id, @p_page_number, @p_page_size, @p_sorting)";

                    using (var cmd = new Npgsql.NpgsqlCommand(query, conn))
                    {
                        // Convert frontend offset to page number
                        int pageNumber = pageOffset / Math.Max(pageSize, 1) + 1;

                        object tenantIdParam = AbpSession.TenantId != null ? (object)AbpSession.TenantId : DBNull.Value;
                        object sortingParam = string.IsNullOrWhiteSpace(sorting) ? (object)"Id" : (object)sorting;

                        cmd.Parameters.AddWithValue("p_tenant_id", tenantIdParam);
                        cmd.Parameters.AddWithValue("p_page_number", pageNumber);
                        cmd.Parameters.AddWithValue("p_page_size", pageSize);
                        cmd.Parameters.AddWithValue("p_sorting", sortingParam);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dto = DataReaderMapper.MapEvaluationsPSQL(reader);
                                lstGetEvaluationViewDto.Add(dto);

                                // Capture total count once
                                if (totalCount == 0 && !reader.IsDBNull(reader.GetOrdinal("totalcount")))
                                {
                                    totalCount = reader.GetInt32(reader.GetOrdinal("totalcount"));
                                }
                            }
                        }
                    }
                }

                return lstGetEvaluationViewDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateOrEdit(CreateOrEditEvaluationDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        public async Task Delete(EntityDto<long> input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            using (var conn = new Npgsql.NpgsqlConnection(_postgresConnection))
            {
                await conn.OpenAsync();

                using (var cmd = new Npgsql.NpgsqlCommand("SELECT dbo.evaluation_delete(@p_id)", conn))
                {
                    cmd.Parameters.AddWithValue("p_id", input.Id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAll()
        {
            await using var conn = new NpgsqlConnection(_postgresConnection);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT dbo.delete_all_evaluations();", conn);
            await cmd.ExecuteNonQueryAsync();
        }




        public async Task<GetEvaluationForEditOutput> GetEvaluationForEdit(EntityDto<long> input)
        {
            var evaluation = await _evaluationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEvaluationForEditOutput { evaluation = new CreateOrEditEvaluationDto
            {
               
                ContactName = evaluation.ContactName,
                CreationTime = evaluation.CreationTime,
                EvaluationsText = evaluation.EvaluationsText,

                OrderId = evaluation.OrderId,
                OrderNumber = evaluation.OrderNumber,
                 Id= input.Id,
               
                PhoneNumber = evaluation.PhoneNumber
            }
            };

            return output;
        }

        public async Task<GetEvaluationForViewDto> GetEvaluationForView(long id)
        {
            var evaluation = await _evaluationRepository.GetAsync(id);

            var output = new GetEvaluationForViewDto { evaluation =new EvaluationDto
            {
                 TenantId= (int?)AbpSession.TenantId,
                ContactName = evaluation.ContactName,
                CreationTime = evaluation.CreationTime,
                EvaluationsText = evaluation.EvaluationsText,

                OrderId = evaluation.OrderId,
                OrderNumber = evaluation.OrderNumber,
                Id = id,
               
                PhoneNumber = evaluation.PhoneNumber
            }
            };

            return output;
        }


        protected virtual async Task Create(CreateOrEditEvaluationDto input)
        {


            try
            {
                var SP_Name = Constants.Evaluation.SP_EvaluationAdd;

                var sqlParameters = new List<SqlParameter>(){
                    new SqlParameter("@ContactName", input.ContactName),
                   new SqlParameter("@CreationTime", input.CreationTime),
                   new SqlParameter("@EvaluationsText", input.EvaluationsText),
                   new SqlParameter("@OrderId", input.OrderId),
                   new SqlParameter("@OrderNumber", input.OrderNumber),
                   new SqlParameter("@PhoneNumber", input.PhoneNumber),
                   new SqlParameter("@TenantId", input.TenantId),
                   new SqlParameter("@EvaluationRate", input.EvaluationRate),
                    new SqlParameter("@EvaluationsReat", input.EvaluationsReat),
                    };


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);




            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        protected virtual async Task Update(CreateOrEditEvaluationDto input)
        {
        

            Evaluation evaluation = new Evaluation
            {
                Id= (long)input.Id,
                ContactName = input.ContactName,
                CreationTime = input.CreationTime,
                EvaluationsText = input.EvaluationsText,

                OrderId = input.OrderId,
                OrderNumber = input.OrderNumber,

               
                PhoneNumber = input.PhoneNumber

            };
            if (AbpSession.TenantId != null)
            {
                evaluation.TenantId = (int?)AbpSession.TenantId;
            }

            await _evaluationRepository.UpdateAsync(evaluation);
        }


        public async Task<FileDto> GetEvaluationsToExcel(GetAllEvaluationsInput input)
        {
            try
            {
                input.SkipCount = 0;
                input.MaxResultCount = int.MaxValue;

                List<GetEvaluationForViewDto> evaluationsList = new List<GetEvaluationForViewDto>();

                using (var conn = new Npgsql.NpgsqlConnection(_postgresConnection))
                {
                    await conn.OpenAsync();

                    string query = @"
            SELECT e.*, c.creatoruserid AS customer_userid
            FROM dbo.evaluation_get(
                @p_tenant_id,
                1,  -- page number for full export
                2147483647,  -- fetch all rows
                @p_sorting,
                @filter,
                @nameFilter
            ) e
            LEFT JOIN dbo.customers c
                ON TRIM(e.phonenumber) = TRIM(c.phonenumber)
            ORDER BY " + (string.IsNullOrWhiteSpace(input.Sorting) ? "id ASC" : input.Sorting);

                    using (var cmd = new Npgsql.NpgsqlCommand(query, conn))
                    {
                        object tenantIdParam = AbpSession.TenantId != null ? (object)AbpSession.TenantId : DBNull.Value;
                        object filterParam = string.IsNullOrWhiteSpace(input.Filter) ? DBNull.Value : (object)input.Filter;
                        object nameFilterParam = string.IsNullOrWhiteSpace(input.NameFilter) ? DBNull.Value : (object)input.NameFilter;

                        cmd.Parameters.AddWithValue("p_tenant_id", tenantIdParam);
                        cmd.Parameters.AddWithValue("p_sorting", input.Sorting ?? "id ASC");
                        cmd.Parameters.AddWithValue("filter", filterParam);
                        cmd.Parameters.AddWithValue("nameFilter", nameFilterParam);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var dto = DataReaderMapper.MapEvaluations(reader);

                                dto.UserId = reader.IsDBNull(reader.GetOrdinal("customer_userid"))
                                    ? ""
                                    : reader.GetString(reader.GetOrdinal("customer_userid"));

                                evaluationsList.Add(dto);
                            }
                        }
                    }
                }

                Debug.WriteLine("Exporting evaluations to Excel...");
                foreach (var eval in evaluationsList)
                {
                    Debug.WriteLine("Id: ==========================");
                }

                return _evaluationsExcelExporter.ExportToFile(evaluationsList);
            }
            catch (Exception ex) {
                Debug.WriteLine("33333333333333333333333333333333333333333333333333333s");
                Debug.WriteLine(ex);
                return new FileDto();
            }
        }


    }
}