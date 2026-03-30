using System.Collections.Generic;
using AttractionCatalog.Domain.Core.Attractions.Aggregates;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.ValueObjects;

namespace AttractionCatalog.Domain.Core.Attractions.Ports
{
    public interface IAttractionRepository
    {
        void Save(IAttractionComponent attraction);
        IAttractionComponent FindById(AttractionId id);
        List<IAttractionComponent> FindByCriteria(IQuerySpecification<IAttractionComponent> spec);
    }
}
