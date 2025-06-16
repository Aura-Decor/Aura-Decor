namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureSpecParams
{
    
    public string? sort { get; set; }
    public Guid? brandId { get; set; }
    public Guid? categoryId { get; set; }
    public Guid? StyleTypeId { get; set; }
    public Guid? ColorId { get; set; }


    public string? search { get; set; }
    
    private const int MaxPageSize = 10;
    private int _pageSize = 5  ;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > 50) ? MaxPageSize : value;
    }
    public int PageIndex { get; set; } = 1;

    private string? _search;
    public string? Search
    {
        get => _search;
        set => _search = value.ToLower();
    }
    
    
}