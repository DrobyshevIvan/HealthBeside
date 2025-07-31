namespace HealthBeside.Domain.Exceptions;

public class MarketReviewException(string error) : Exception($"Market review error: {error}");