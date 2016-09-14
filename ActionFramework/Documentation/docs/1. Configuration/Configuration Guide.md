Configure global settings, available for all actions

Using Exec (configure executing methods) for configuration values

Using variables for configuration values

<pre class="language-markup">
  <code>
  <!-- using an invoke method to get the current runtime parent uri -->
  <Setting Name="ExecutionRoot" Value="Invoke|GetExecutionParentRoot" />

  <!-- using a variable {ExecutionRoot} from another Setting/Property -->
  <Setting Name="SourceRootFolder" Value="{ExecutionRoot}source\" />
  <Setting Name="LogFile" Value="{ExecutionRoot}Logs\{GetCurrentFormatDateTimeString}.log.xml" />

</Settings>
  </code>
</pre>

Configure a runtime property value from another action (Exec)

<pre class="language-markup">
  <code>
<Action Id="1" Type="GetCustomers" Enabled="false" ClientExecute="false" Description="Get all customers" BreakOnError="true" />

<Action Id="2" Type="ReturnXml" Enabled="false" ClientExecute="true" Description="xml result values" BreakOnError="true">
    <Property Name="Customers" Value="Exec|GetCustomers" />
</Action>
</code>
</pre>
