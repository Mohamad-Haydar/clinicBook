using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace API.Tests.Controllers;

public class ModelStateTestController : Controller
{
    public ModelStateTestController()
    {
        ControllerContext = Substitute.For<ControllerContext>();
    }

    public bool TestTryValidateModel(object model)
    {
        return TryValidateModel(model);
    }
}