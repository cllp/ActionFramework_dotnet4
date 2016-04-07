# Action Framework #

### Action Framework is an integration and orchestration platform that enables a simple process to develop, manage, monitor and extend systems, integrations and data flows in complex environments, all managed through a dashboard that gives administrators control of the infrastructure ###


* Action Framework enables analysis and monitoring of information that was previously very complex or maybe not even possible.


* Action Framework provides insights that will help organizations to more productivity,profitability and creates security and control over the infrastructure regardless of whether the systems are cloud-based or on-premises.


* Action Framework enables development with common programming languages based on modern platforms with minimal dependencies.



*With the Action Framework, we offer an alternative to the variety of different solutions that exist in areas such as integration and introduces a new and especially easy way to continuous develop integrations with great overview and security of the infrastructure, we enable the opportunities of Cloud and thereby increase productivity and dramatically reduce complexity.*


### End user value - IT Managers / Purchasing / Decisions ###

* Access to library of agents and apps based on other AF implementations
* Reduced implementation costs for end user
* Access to Cloud based test environment for any new integration or addition of functionality through AF Cloud environment
* Combinations of environments - On Premise, Cloud, Private Cloud or PaaS (Platform as a service) Servers and agents can easily be moved between environments and locations
* Quick and easy installation - ”One click install" to deploy new servers, agents and apps
* No need for change or replace existing systems or infrastructure
* No major business decisions to initiate an introduction
* The platform can increase within the organization along with new demands as needs arise Integration solution based on AF can be used outside the framework
* Native support for customized logging and "Widgets" for specific individual purposes
* Configurable dashboards based on role and individual interests
* Built in reports and statistics for integrations and system extensions

### End user value - IT Department / Integration Team ###

* Overview of all systems integrations and system extensions
* Easy to extend existing systems, without vendor dependency and expensive changes Standardized configuration and maintenance of integrations and applications
* AF agents and servers communicates through standard protocols and ports (encrypted HTTP) Configure the entire organization integrations from standardized toolset
* Monitoring of system integration - Complete integration pulse (no black box)
* Logging - status, alarms, information
* Centralized configuration/deployment of new applications
* Integrations in "Test Mode"
* Complete control for agents and execution flows

### End user value - IT Department / Development ###

* Consistent process for the development of integrations and new functionality
* All integrations follow the same structure
* Easy to create new applications using standard programming languages
* Requires no special skills to further develop and create integrations
* Organizations can use their own developers for new integrations or further development Testable integrations, all apps can run in ”testmode” by default

### General Architecture ###

* Platform-independent 
* Built with .NET Framework.  
* AF consists of a core (Action Framework Core) and of
two main components: ”AF Agent" and ”AF Server".  
* Agent AF and AF Server serves its own RESTful API ́s
and communicates through HTTP only.  
* The agents are administered and configured from the
associated server instance.  
* Each agent sends activity data to the associated server
instance.  
* The components can be run in a variety of environments and platforms without the need for external applications or web servers.  
* The architecture enables installation of "apps" and added functionality with simple administration tasks from the server portal.

![actionframework.png](https://bitbucket.org/repo/b9gBGB/images/1689978544-actionframework.png)

# Agents #

* Service that runs on any location within the organization's infrastructure or as a
cloud service. 
* Agent execute various functions within the installed "apps" 
* The agent retrieves "run-schedule" from a server instance. 
* The agent sends the meta data and data in real time to a server instance. 
* Agent's "timer", ie how often it runs, administered from a server instance.

![af_serverinstance.png](https://bitbucket.org/repo/b9gBGB/images/497627914-af_serverinstance.png)

# Apps #

* An app is a software component (assembly) being developed for a specific purpose
* The component based on standardized programming languages, ie developers need no specific knowledge of the AF to develop a new app, see [howto](https://bitbucket.org/cllp/actionframework/wiki/Howto%20-%20create%20an%20action) for more info.
* Apps are configured in the agent's "run-schedule"
* The app has built-in logging and sends the meta-data and application data to a
server instance. 

### Connectors ###

* Connectors are mainly a library of components and a toolset for app developers and used to interact with different types of platforms and services,.  
* Reusing connection components (Connectors) for interaction with various business applications and cloud services will minimize development time, cost, complexity and ensures stability.

![af_app.png](https://bitbucket.org/repo/b9gBGB/images/2401256862-af_app.png)
