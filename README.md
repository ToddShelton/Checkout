## Checkout -- an actor-based service composition

This subset of a larger application demonstrates <a href="https://github.com/dotnet/Orleans">Project Orleans</a> actors used to compose and manage an e-commerce cart.  The hosting silos are not shown, so this code won't run as is.  You can compile it though, once the NuGet packages download and install.  

#### Overview
I became interested in actors when I discovered Microsoft's Project Orleans last summer. Abstracting away the difficulty of low-level distributed cloud applications in Azure was compelling, and I rewrote an e-commerce app to take advantage of Orleans' scalable, resilient silo/grain architecture.  

I like that Orleans silos can hold response time at >90% CPU utilization (says Microsoft, I haven't tested yet) and once built the silos are easy to deploy onto Azure.  

#### Actors as a gateway to Azure microservices 

Virtual machines are today's most popular cloud platform, but Mark Russinovich has already said that<a href="http://www.zdnet.com/article/whats-next-for-microsofts-platform-as-a-service/">  Microsoft will transition the Azure web role/worker role PaaS platform into a more microservice-oriented form</a>.  

Actors' encapsulation, inherent scalability, and fault-tolerance seem like a natural form to run on a microservices platform.  

## Code highlights

In this application grains prefixed `ServiceGrain` interact directly with third party services.  They are all REST-based packages from NuGets except Avalara who don't publish NuGets yet.  .NET or WCF connections work fine too. Grains prefixed `Grain` aggregate service grains with business logic to compose complex services. 

The unit tests run against grains running in live silos, so the release progression goes from development grain/silo to production grain/silo.  

At this moment versioning grains requires a full rebuild of all the silos (as opposed to a rolling upgrade) but I am told this will change.  Smart people have developed grain versioning techniques that they say work fine in production. The approach is to use `OnActivateAsync()` (see below) to compare the grain's version with the current version, and upgrade the schema of that grain if its version is out of date. 

`ServiceGrainBingLocation` shows a from-scratch REST client.  The other service grains use HTTP connections that are built in to their respective SDKs.

#### Serializing grain-to-grain communication

Because actors communicate by passing messages, types passed between grains have to be serializable.  Many .NET classes are not serializable and can't be used for grain-to-grain communication. A solution is to <a href="http://orleans.codeplex.com/discussions/611776">create a covering class that the grains can use, then copy its properties into the non-serializable class</a>.  

The `IServiceGrainSendGrid` interface and its companion grain `ServiceGrainSendGrid` are a good example.  The `SendGridMessage` class comes with the SendGrid SDK, and it uses `System.Net.Mail.MailAddress`, which is not serializable.  The `EmailMessage` class is, and passes the email message to the SendGrid service grain, which copies its properties into the `SendGridMessage` class and sends it off to SendGrid.  

#### Actors are asynchronous 

Actors can scale because they are single-threaded internally, and their methods are asynchronous.  In .NET, that means all grain methods are async methods that return `Task` or `Task<T>`.  You can see examples of both in `GrainCart`.  

Overriding the `OnActivateAsync()` grain method is a way to initialize a grain or read its state from a persisted state provider. `ServiceGrainAvalara` and `ServiceGrainSendGrid` demonstrate initialization, and `GrainCart` demonstrates reading state from a provider, if that state exists.  

#### Conclusion

Microservice platforms promise resilient, automatically scalable service compositions that can stay up, seek and serve demand, respond to outages, and use cloud computing dollars as efficiently as possible.   

For application architectures that can use them effectively, actors are a powerful and convenient way to compose and deliver microservice-based applications.  

### Credits and thanks

<a href="http://research.microsoft.com/en-us/people/sbykov/">Sergey Bykov</a> and <a href="http://research.microsoft.com/en-us/people/gkliot/">Gabriel Kliot</a> have been very generous with their time and have been instrumental in helping me to understand and build code using this very cool technology.   










