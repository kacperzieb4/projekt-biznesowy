using System.Collections.Concurrent;
using TouristManagement.Domain.Core.Attractions;
using TouristManagement.Domain.Modules.CatalogSearch;

namespace TouristManagement.Infrastructure.Persistence;

public interface IQuerySpecification<T> { Predicate<T> ToExpression(); }

public interface IAttractionRepository
{
    void Save(IAttractionComponent attraction);
    IAttractionComponent FindById(AttractionId id);
    List<IAttractionComponent> FindByCriteria(IQuerySpecification<IAttractionComponent> spec);
    List<IAttractionComponent> GetAll();
}

public class InMemoryAttractionRepository : IAttractionRepository
{
    private readonly ConcurrentDictionary<AttractionId, IAttractionComponent> _data = new();

    public void Save(IAttractionComponent attraction) => _data[attraction.Id] = attraction;
    public IAttractionComponent FindById(AttractionId id) => _data.GetValueOrDefault(id);
    public List<IAttractionComponent> GetAll() => _data.Values.ToList();

    public List<IAttractionComponent> FindByCriteria(IQuerySpecification<IAttractionComponent> spec)
    {
        var predicate = spec.ToExpression();
        return _data.Values.Where(x => predicate(x)).ToList();
    }
}

public class LocationQuerySpecification : IQuerySpecification<IAttractionComponent>
{
    private readonly GeoArea _area;
    public LocationQuerySpecification(GeoArea area) => _area = area;

    public Predicate<IAttractionComponent> ToExpression() => (comp) =>
    {
        if (comp is SingleAttraction single)
        {
            // Uproszczone wyliczanie dystansu (wersja PoC)
            var dLat = single.Location.Latitude - _area.CenterLatitude;
            var dLon = single.Location.Longitude - _area.CenterLongitude;
            var distance = Math.Sqrt(dLat * dLat + dLon * dLon) * 111;
            return distance <= _area.RadiusKm;
        }
        return true; 
    };
}