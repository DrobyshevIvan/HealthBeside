namespace HealthBeside.Domain.Models.Marketplace;

public class MarketProduct
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public string SKU { get; private set; }

    public string? ImageUrl { get; private set; }

    public Guid CategoryId { get; private set; }
    public MarketCategory Category { get; private set; }

    public ICollection<MarketOrderItem> OrderItems { get; private set; }
    public ICollection<MarketCartItem> CartItems { get; private set; }
    public ICollection<MarketReview> Reviews { get; private set; }

    private MarketProduct()
    {
    }

    public static (string? Error, MarketProduct MarketProduct) Create(string name, string description, decimal price,
        int quantity, string sku, string imageUrl, Guid categoryId)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            errors.Add("Description cannot be empty.");

        if (price <= 0)
            errors.Add("Price must be greater than zero.");

        if (quantity <= 0)
            errors.Add("Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(sku))
            errors.Add("SKU cannot be empty.");

        if (categoryId.Equals(Guid.Empty))
            errors.Add("Category ID cannot be empty.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var marketProduct = new MarketProduct
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Quantity = quantity,
            SKU = sku,
            ImageUrl = imageUrl,
            CategoryId = categoryId
        };

        return (null, marketProduct);
    }

    public string? Update(string? name, string? description, decimal? price, int? quantity, string? sku,
        string? imageUrl, Guid? categoryId)
    {
        bool hasChanges = false;
        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(name) && name != Name)
        {
            Name = name;
            hasChanges = true;
        }
        else if (name is not null && string.IsNullOrWhiteSpace(name))
        {
            errors.Add("Name cannot be empty.");
        }

        if (!string.IsNullOrWhiteSpace(description) && description != Description)
        {
            Description = description;
            hasChanges = true;
        }
        else if (description is not null && string.IsNullOrWhiteSpace(description))
        {
            errors.Add("Description cannot be empty.");
        }

        if (price.HasValue)
        {
            if (price.Value < 0)
            {
                errors.Add("Price must be greater than zero.");
            }
            else if (price.Value != Price)
            {
                Price = price.Value;
                hasChanges = true;
            }
        }

        if (quantity.HasValue)
        {
            if (quantity.Value < 0)
            {
                errors.Add("Quantity must be greater than zero.");
            }
            else if (quantity.Value != Quantity)
            {
                Quantity = quantity.Value;
                hasChanges = true;
            }
        }

        if (!string.IsNullOrWhiteSpace(sku) && sku != SKU)
        {
            SKU = sku;
            hasChanges = true;
        }
        else if (sku is not null && string.IsNullOrWhiteSpace(sku))
        {
            errors.Add("SKU cannot be empty.");
        }

        if (categoryId.HasValue)
        {
            if (categoryId.Value == Guid.Empty)
            {
                errors.Add("Category ID cannot be empty.");
            }
            else if (categoryId.Value != CategoryId)
            {
                CategoryId = categoryId.Value;
                hasChanges = true;
            }
        }

        if (!string.IsNullOrWhiteSpace(imageUrl) && imageUrl != ImageUrl)
        {
            ImageUrl = imageUrl;
            hasChanges = true;
        }

        if (errors.Any())
            return string.Join("; ", errors);

        return null;
    }

    public string? UpdateStock(int newQuantity)
    {
        if (newQuantity < 0)
            return "Cannot reduce quantity below zero.";

        Quantity = newQuantity;
        return null;
    }
}