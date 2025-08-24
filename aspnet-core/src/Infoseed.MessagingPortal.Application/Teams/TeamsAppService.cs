using Framework.Data;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.Departments;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.Group.Dto;
using Infoseed.MessagingPortal.Groups.Dto;
using Infoseed.MessagingPortal.Teams.Dto;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Teams
{
    public class TeamsAppService : MessagingPortalAppServiceBase, ITeamsAppService
    {
        private readonly IDocumentClient _IDocumentClient;
        private readonly IExcelExporterAppService _ExcelExporter;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IDashboardUIAppService _dashboardUIAppService;
        private readonly IUserAppService _iUserAppService;

        public TeamsAppService(IDocumentClient IDocumentClient, IDepartmentAppService departmentAppService, IDashboardUIAppService dashboardUIAppService, IUserAppService iUserAppService, IExcelExporterAppService ExcelExporter = null)
        {
            _IDocumentClient=IDocumentClient;
            _ExcelExporter = ExcelExporter;
            _departmentAppService = departmentAppService;
            _dashboardUIAppService = dashboardUIAppService;
            _iUserAppService=iUserAppService;

        }

        public async Task<TeamsCreateDto> TeamsCreateMembers(TeamsMembersDto input)
        {
            TeamsCreateDto teamsModel = new TeamsCreateDto();
            try
            {
                if (input.teamsModel.TeamsName.Length <= 20)
                {
                    var model = TeamsGetAll("", 0, int.MaxValue)
                                    .TeamsDtoModel
                                    .Where(x => x.TeamsName.ToLower().Trim() == input.teamsModel.TeamsName.ToLower().Trim())
                                    .FirstOrDefault();

                    if (input.isCreate)
                    {
                        if (model == null)
                        {
                            teamsModel = await createMembers(input);
                            if (teamsModel != null)
                            {
                                teamsModel.state = 2;
                                teamsModel.message = "Ok";
                            }
                            else
                            {
                                teamsModel.state = 3;
                                teamsModel.message = "Tenant missing";
                            }
                        }
                        else
                        {
                            teamsModel.state = 1;
                            teamsModel.message = "The Teams name is used";
                        }

                    }
                    else
                    {
                        teamsModel = await editMembers(input);
                        teamsModel.state = 2;
                        teamsModel.message = "Ok";


                    }
                  //  input.teamsModel.TotolForPrograss=input.membersDto.Count();
                   
                }
                else
                {
                    teamsModel.state = 4;
                    teamsModel.message = "The Teams name is more than 20 characters long";
                }
                return teamsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TeamsModel TeamsDelete(long teamsId)
        {
            TeamsModel TeamsModel = new TeamsModel();
            try
            {
                TeamsModel = teamsDelete(teamsId);
                return TeamsModel;
            }
            catch (Exception ex)
            {
                TeamsModel.state = -1;
                TeamsModel.message = ex.Message;
                return TeamsModel;
            }
        }

        public TeamsModel TeamsGetAll(string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                searchTerm="";
            }

            TeamsModel TeamsModel = new TeamsModel();
            try
            {
                TeamsModel = teamsGetAll(searchTerm, pageNumber, pageSize);
                return TeamsModel;
            }
            catch (Exception ex)
            {
                TeamsModel.state = -1;
                TeamsModel.message = ex.Message;
                return TeamsModel;
            }
        }

        public TeamsMembersDto TeamsGetById(long teamsId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<TeamsLog> TeamsLogGetAll(long teamsId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<TeamsCreateDto> TeamsUpdate(TeamsMembersDto input, bool isExternal, int statusId = 0)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, dynamic> ValidTeamsName(string teamsName,bool isCreate=true)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();


                if (isCreate)
                {

                    if (!string.IsNullOrEmpty(teamsName) && teamsName.Length <= 20)
                    {
                        var model = TeamsGetAll("", 0, int.MaxValue)
                                       .TeamsDtoModel
                                       .Where(x => x.TeamsName.ToLower().Trim() == teamsName.ToLower().Trim())
                                       .FirstOrDefault();

                        if (model == null)
                        { response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", "Ok" } }; }
                        else
                        { response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The group name is used" } }; }
                    }
                    else
                    {
                        response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "The group name must not be empty and must not exceed 20 characters" } };
                    }

                }
                else
                {

                    response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", "Ok" } };

                }

             

                return response;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }



        #region privet 
        private async Task<TeamsCreateDto> createMembers(TeamsMembersDto input)
        {
            TeamsCreateDto teamsModel = new TeamsCreateDto();
            try
            {
                if (AbpSession.TenantId.HasValue)
                {
                    var SP_Name = Constants.Teams.SP_TeamsCreate;
                    int tenantId = AbpSession.TenantId.Value;

                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                        new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                        new System.Data.SqlClient.SqlParameter("@teamsName",input.teamsModel.TeamsName) ,
                        new System.Data.SqlClient.SqlParameter("@creationDate",DateTime.UtcNow.AddHours(3)),
                        new System.Data.SqlClient.SqlParameter("@UserIds",input.teamsModel.UserIds) ,
                          new System.Data.SqlClient.SqlParameter("@TotalNumber",input.teamsModel.UserIds.Split(',').Length) ,
                    };
                    var OutputParameter = new System.Data.SqlClient.SqlParameter
                    {
                        SqlDbType = SqlDbType.BigInt,
                        ParameterName = "@outPut",
                        Direction = ParameterDirection.Output
                    };
                    sqlParameters.Add(OutputParameter);
                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                    long outId = (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
                    return teamsModel;

                }
                teamsModel.id = 0;
                return teamsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<TeamsCreateDto>editMembers(TeamsMembersDto input)
        {
            TeamsCreateDto teamsModel = new TeamsCreateDto();
            try
            {
                if (AbpSession.TenantId.HasValue)
                {
                    var SP_Name = Constants.Teams.SP_TeamsEdit;
                    int tenantId = AbpSession.TenantId.Value;

                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                        new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                         new System.Data.SqlClient.SqlParameter("@teamsId",input.teamsModel.Id) ,
                        new System.Data.SqlClient.SqlParameter("@teamsName",input.teamsModel.TeamsName) ,
                        new System.Data.SqlClient.SqlParameter("@modificationDate",DateTime.UtcNow.AddHours(3)),
                        new System.Data.SqlClient.SqlParameter("@UserIds",input.teamsModel.UserIds) ,
                          new System.Data.SqlClient.SqlParameter("@TotalNumber",input.teamsModel.UserIds.Split(',').Length) ,
                    };
                   
                   
                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                    return teamsModel;

                }
                return teamsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static List<List<T>> SplitList<T>(List<T> list, int numberOfLists)
        {
            int chunkSize = (int)Math.Ceiling((double)list.Count / numberOfLists);

            return Enumerable.Range(0, numberOfLists)
                .Select(i => list.Skip(i * chunkSize).Take(chunkSize).ToList())
                .ToList();
        }
        private TeamsModel teamsGetAll(string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            TeamsModel TeamsModel = new TeamsModel();
            try
            {
                if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "")
                    searchTerm = searchTerm.ToLower();

                var SP_Name = Constants.Teams.SP_TeamsGetAll;
                int tenantId = AbpSession.TenantId.Value;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber) ,
                    new System.Data.SqlClient.SqlParameter("@pageSize",pageSize) ,
                    new System.Data.SqlClient.SqlParameter("@searchTerm",searchTerm)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                TeamsModel.TeamsDtoModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTeams, AppSettingsModel.ConnectionStrings).ToList();


                TeamsModel.state = 2;
                TeamsModel.message = "OK";
                TeamsModel.total = OutputParameter.Value != DBNull.Value ? Convert.ToInt64(OutputParameter.Value) : 0;
                if (TeamsModel.total == 0)
                {
                    TeamsModel.state = 2;
                    TeamsModel.message = "No results found";
                }
                return TeamsModel;
            }
            catch (Exception ex)
            {
                TeamsModel.state = -1;
                TeamsModel.message = ex.Message;
                return TeamsModel;
            }
        }
        private TeamsModel teamsDelete(long id)
        {
            TeamsModel groupModel = new TeamsModel();
            try
            {
                var SP_Name = Constants.Teams.SP_TeamsDelete;
                int tenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@teamsId",id) ,
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                groupModel.state = 2;
                groupModel.message = "OK";
                groupModel.total = OutputParameter.Value != DBNull.Value ? Convert.ToInt64(OutputParameter.Value) : 0;

                if (groupModel.total == 0)
                {
                    groupModel.state = 2;
                    groupModel.message = "No results found";
                }
                return groupModel;
            }
            catch (Exception ex)
            {
                groupModel.state = -1;
                groupModel.message = ex.Message;
                return groupModel;
            }
        }

        #endregion


    }
}
