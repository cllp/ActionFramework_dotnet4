Create an action class that inherits from ActionBase.
<pre class="language-csharp"><code>
using ActionFramework.Base;
using System;

namespace DemoApp
{
    public class DemoAction1 : ActionBase
    {
        //define the runtime configuration properties
        public string MyProp { get; set; }

        //override the runtime "execute" function
        public override object Execute()
        {
            try
            {
                //implement action logic, log my "MyProp" value
                Log.Info("Just make a log entry: " + MyProp);

                return HandleSuccess();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
</code>
</pre>
Configure the action.
<pre class="language-markup">
<code>
<?xml version="1.0" encoding="UTF-8"?>

<ActionFramework>
    <Settings>
        <Setting Value="true" Name="MyGlobalProperty"/>

        <!-- define a runtime function "GetExecutionParentRoot" -->
        <Setting Value="Invoke|GetExecutionParentRoot" Name="ExecutionRoot"/>

        <!-- using {variables} to define a custom log folder -->
        <Setting Value="{ExecutionRoot}log\custom\" Name="CustomLogFolder"/>
    </Settings>

    <Actions>
        <Action Id="1" Type="DemoAction1" BreakOnError="true" Description="A short description" ClientExecute="true" Enabled="true">
            <Property Name="MyProp" Value="{CustomLogFolder}"/>
        </Action>
    </Actions>

</ActionFramework>
</code>
</pre>

Log output (xml).

<pre class="language-markup">
<code>
<ActionLog Created="2015-12-02T14:00:55.5684275+01:00">
  <Log>

    <!-- action log results -->
    <Action ActionId="2" ActionType="DemoAction1" Created="2015-12-02T13:59:43.1744245+01:00" Assembly="DemoApp v.1.0.0.0 by Woxion AB">
      <Information>Just make a log entry: C:\actionframework\agents\12345\log\custom\</Information>
      <Information>Status: OK</Information>
    </Action>

    <!-- general agent information log -->
    <Agent Created="2015-12-02T13:58:03.2254255+01:00">
      <AssemblyLog>
        <Message>Assembly</Message>
        <Version>
          <Major>1</Major>
          <Minor>0</Minor>
          <Build>5814</Build>
          <Revision>25042</Revision>
          <MajorRevision>0</MajorRevision>
          <MinorRevision>25042</MinorRevision>
        </Version>
        <Title>ActionFramework</Title>
        <ProductName>ActionFramework</ProductName>
        <CopyrightHolder>Copyright © Woxion AB 2013</CopyrightHolder>
        <CompanyName>Woxion AB</CompanyName>
      </AssemblyLog>

      <!-- logging server instance url -->
      <Information>ActionList Initiated. Using http://localhost:12345 server url.</Information>

      <!-- general log results -->
      <ActionResultLog>
        <Message>Action Results</Message>
        <Total>1</Total>
        <AgentExecute>1</AgentExecute>
        <InternalActionExecute>0</InternalActionExecute>
        <Failed>0</Failed>
        <Runtime>00:01:12.39</Runtime>
      </ActionResultLog>
    </Agent>
  </Log>
</ActionLog>
</code>
</pre>
