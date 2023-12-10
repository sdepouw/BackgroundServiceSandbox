using Autofac;
using Core7Library.BearerTokenStuff;
using Core7Library.CatFacts;

namespace Core7Library;

public class Core7LibraryAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CatFactsClientService>().As<ICatFactsClientService>();
    }
}
