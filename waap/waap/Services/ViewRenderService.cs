using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace waap.Services
{
    public class ViewRenderService
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync(ControllerContext controllerContext, string viewName, object model)
        {
            var viewEngineResult = _viewEngine.FindView(controllerContext, viewName, false);
            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"A view '{viewName}' não foi encontrada.");
            }

            var view = viewEngineResult.View;
            using var sw = new StringWriter();
            var viewData = new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };
            var viewContext = new ViewContext(
                controllerContext,
                view,
                viewData,
                new TempDataDictionary(controllerContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await view.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}