namespace TradingPlatform.Application.Exceptions;

public class AppException(string message) : Exception(message);

public class NotFoundException(string message) : AppException(message);

public class ValidationException(string message) : AppException(message);

public class UnauthorizedException(string message) : AppException(message);
