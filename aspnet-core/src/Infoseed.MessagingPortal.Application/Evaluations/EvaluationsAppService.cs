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

namespace Infoseed.MessagingPortal.Evaluations
{
    public class EvaluationsAppService : MessagingPortalAppServiceBase, IEvaluationsAppService
	{

        private readonly IEvaluationExcelExporter _evaluationsExcelExporter;
        private readonly IRepository<Evaluation, long> _evaluationRepository;
        private readonly IRepository<Contact> _lookup_customerRepository;

        public EvaluationsAppService(IRepository<Evaluation, long> evaluationRepository, IEvaluationExcelExporter evaluationsExcelExporter, IRepository<Contact> lookup_customerRepository)
		{
			_evaluationRepository = evaluationRepository;

            _lookup_customerRepository=lookup_customerRepository;
            _evaluationsExcelExporter = evaluationsExcelExporter;
        }
        public EvaluationsAppService()
        {

        }
        public async Task<PagedResultDto<GetEvaluationForViewDto>> GetAll(GetAllEvaluationsInput input)
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

        private List<GetEvaluationForViewDto> GetALLEvaluation(int pageNumber, int pageSize, string sorting, out int totalCount)
        {
            try
            {
                List<GetEvaluationForViewDto> lstGetEvaluationViewDto = new List<GetEvaluationForViewDto>();
                var SP_Name = Constants.Evaluation.SP_EvaluationGet; ;


                var sqlParameters = new List<SqlParameter> {
                        new SqlParameter("@PageNumber",pageNumber)
                       ,new SqlParameter("@PageSize",pageSize)
                       ,new SqlParameter("@TenantId",(int?)AbpSession.TenantId)
                       ,new SqlParameter("@Sorting",sorting)
                 };

                SqlParameter OutsqlParameter = new SqlParameter();
                OutsqlParameter.ParameterName = "@TotalCount";
                OutsqlParameter.SqlDbType = System.Data.SqlDbType.Int;
                OutsqlParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutsqlParameter);
                lstGetEvaluationViewDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.MapEvaluations, AppSettingsModel.ConnectionStrings).ToList();
                totalCount = (int)OutsqlParameter.Value;
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
            await _evaluationRepository.DeleteAsync(input.Id);
        }

        public async Task DeleteAll()
        {
            var Evaluations = _evaluationRepository.GetAll().ToList();

            foreach(var eva in Evaluations)
            {
                await _evaluationRepository.DeleteAsync(eva.Id);
            }
           
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
           
            input.SkipCount = 0;
            input.MaxResultCount = int.MaxValue;

            var filteredEvaluations = _evaluationRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ContactName.Contains(input.Filter))
                 .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.EvaluationsText == input.NameFilter);






            var pagedAndFilteredOrders = filteredEvaluations
            .OrderBy(input.Sorting ?? "id asc")
            .PageBy(input);

            var evaluation = from o in pagedAndFilteredOrders
                             join o3 in _lookup_customerRepository.GetAll() on o.PhoneNumber.Trim() equals o3.PhoneNumber.Trim() into j3
                             from s3 in j3.DefaultIfEmpty()

                             select new GetEvaluationForViewDto()
                             {
                                 evaluation = new EvaluationDto
                                 {
                                     Id = o.Id,
                                     OrderNumber = o.OrderNumber,
                                     ContactName = o.ContactName,
                                      EvaluationsReat=o.EvaluationsReat,
                                     EvaluationsText = o.EvaluationsText,
                                     OrderId = o.OrderId,
                                     PhoneNumber = o.PhoneNumber,
                                     TenantId = o.TenantId,
                                     CreationTime = o.CreationTime
                                 },
                                 CreatTime = o.CreationTime.ToString("hh:mm tt"),
                                 CreatDate = o.CreationTime.ToString("MM/dd/yyyy"),
                                 UserId = s3 == null || s3.PhoneNumber == null ? "" : s3.UserId.ToString(),

                             };





            var list = evaluation.ToList();
              return _evaluationsExcelExporter.ExportToFile(list);
           // return null;
        }
    }
}