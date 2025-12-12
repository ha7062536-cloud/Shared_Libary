using Xunit;
using Shared.Http;
using System.Collections;
using System.Collections.Specialized;
using System.Net;

namespace Shared.Test;

public class HttpRouterTests
{
	[Fact]
	public void Constructor_InitializesEmptyRouter()
	{
		// Act
		var router = new HttpRouter();

		// Assert
		Assert.NotNull(router);
	}

	[Fact]
	public void Use_AddsMiddlewaresToRouter()
	{
		// Arrange
		var router = new HttpRouter();
		HttpMiddleware middleware = async (req, res, props, next) => await next();

		// Act
		var result = router.Use(middleware);

		// Assert
		Assert.Same(router, result); // Fluent API
	}

	[Fact]
	public void Map_AddsRouteWithMethod()
	{
		// Arrange
		var router = new HttpRouter();
		HttpMiddleware middleware = async (req, res, props, next) => await next();

		// Act
		var result = router.Map("GET", "/test", middleware);

		// Assert
		Assert.Same(router, result); // Fluent API
	}

	[Fact]
	public void MapGet_CreatesGetRoute()
	{
		// Arrange
		var router = new HttpRouter();
		HttpMiddleware middleware = async (req, res, props, next) => await next();

		// Act
		var result = router.MapGet("/test", middleware);

		// Assert
		Assert.Same(router, result);
	}

	[Fact]
	public void MapPost_CreatesPostRoute()
	{
		// Arrange
		var router = new HttpRouter();
		HttpMiddleware middleware = async (req, res, props, next) => await next();

		// Act
		var result = router.MapPost("/test", middleware);

		// Assert
		Assert.Same(router, result);
	}

	[Fact]
	public void MapPut_CreatesPutRoute()
	{
		// Arrange
		var router = new HttpRouter();
		HttpMiddleware middleware = async (req, res, props, next) => await next();

		// Act
		var result = router.MapPut("/test", middleware);

		// Assert
		Assert.Same(router, result);
	}

	[Fact]
	public void MapDelete_CreatesDeleteRoute()
	{
		// Arrange
		var router = new HttpRouter();
		HttpMiddleware middleware = async (req, res, props, next) => await next();

		// Act
		var result = router.MapDelete("/test", middleware);

		// Assert
		Assert.Same(router, result);
	}

	[Fact]
	public void Use_AllowsMultipleMiddlewares()
	{
		// Arrange
		var router = new HttpRouter();
		HttpMiddleware middleware1 = async (req, res, props, next) => await next();
		HttpMiddleware middleware2 = async (req, res, props, next) => await next();

		// Act
		var result = router.Use(middleware1, middleware2);

		// Assert
		Assert.Same(router, result);
	}

	[Fact]
	public void UseRouter_AddsSubRouter()
	{
		// Arrange
		var mainRouter = new HttpRouter();
		var subRouter = new HttpRouter();

		// Act
		var result = mainRouter.UseRouter("/api", subRouter);

		// Assert
		Assert.Same(mainRouter, result);
	}

	[Fact]
	public void UseSimpleRouteMatching_EnablesSimpleMatching()
	{
		// Arrange
		var router = new HttpRouter();

		// Act
		var result = router.UseSimpleRouteMatching();

		// Assert
		Assert.Same(router, result);
	}

	[Fact]
	public void UseParametrizedRouteMatching_EnablesParametrizedMatching()
	{
		// Arrange
		var router = new HttpRouter();

		// Act
		var result = router.UseParametrizedRouteMatching();

		// Assert
		Assert.Same(router, result);
	}

	[Fact]
	public void ParseUrlParams_WithMatchingSimplePath_ReturnsEmptyParameters()
	{
		// Arrange
		string urlPath = "/api/movies";
		string routePath = "/api/movies";

		// Act
		var result = HttpRouter.ParseUrlParams(urlPath, routePath);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public void ParseUrlParams_WithParametrizedPath_ReturnsParameters()
	{
		// Arrange
		string urlPath = "/api/movies/42";
		string routePath = "/api/movies/:id";

		// Act
		var result = HttpRouter.ParseUrlParams(urlPath, routePath);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("42", result["id"]);
	}

	[Fact]
	public void ParseUrlParams_WithMultipleParameters_ReturnsAllParameters()
	{
		// Arrange
		string urlPath = "/api/users/123/posts/456";
		string routePath = "/api/users/:userId/posts/:postId";

		// Act
		var result = HttpRouter.ParseUrlParams(urlPath, routePath);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("123", result["userId"]);
		Assert.Equal("456", result["postId"]);
	}

	[Fact]
	public void ParseUrlParams_WithMismatchedPathLength_ReturnsNull()
	{
		// Arrange
		string urlPath = "/api/movies/42";
		string routePath = "/api/movies";

		// Act
		var result = HttpRouter.ParseUrlParams(urlPath, routePath);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void ParseUrlParams_WithNonMatchingSegment_ReturnsNull()
	{
		// Arrange
		string urlPath = "/api/movies/42";
		string routePath = "/api/users/:id";

		// Act
		var result = HttpRouter.ParseUrlParams(urlPath, routePath);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void ParseUrlParams_WithUrlEncodedParameter_DecodesParameter()
	{
		// Arrange
		string urlPath = "/api/search/hello%20world";
		string routePath = "/api/search/:query";

		// Act
		var result = HttpRouter.ParseUrlParams(urlPath, routePath);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("hello world", result["query"]);
	}

	[Fact]
	public void ParseUrlParams_WithTrailingSlashes_HandlesCorrectly()
	{
		// Arrange
		string urlPath = "/api/movies/42/";
		string routePath = "/api/movies/:id/";

		// Act
		var result = HttpRouter.ParseUrlParams(urlPath, routePath);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("42", result["id"]);
	}

	[Fact]
	public void ParseUrlParams_WithEmptyParameters_ReturnsEmpty()
	{
		// Arrange
		string urlPath = "/";
		string routePath = "/";

		// Act
		var result = HttpRouter.ParseUrlParams(urlPath, routePath);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public void RESPONSE_NOT_SENT_ConstantHasCorrectValue()
	{
		// Assert
		Assert.Equal(777, HttpRouter.RESPONSE_NOT_SENT);
	}
}
