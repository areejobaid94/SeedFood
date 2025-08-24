using Abp.Application.Services;

namespace Infoseed.MessagingPortal.Generic
{
    public interface IGenericAppService<T> :  IApplicationService 
    {
        T GetAll(string searchTerm = null, int? pageNumber = 0, int? pageSize = 10);
        T GetById(long id);
        T Create(T input);
        T Update(T input);
        T Delete(long id);
    }
}
