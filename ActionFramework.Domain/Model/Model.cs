using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Woxion.Utility.ActionFramework.Domain.Model
{
    [DataContract]
    public partial class Action
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int AgentId { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public bool ClientExecute { get; set; }
        [DataMember]
        public bool BreakOnError { get; set; }
        [DataMember]
        public int? AppId { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Description { get; set; }
    }

    [DataContract]
    public partial class AgentApp
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int AgentId { get; set; }
        [DataMember]
        public bool Installed { get; set; }
        [DataMember]
        public System.DateTime? InstallDate { get; set; }
        [DataMember]
        public int? AppId { get; set; }
    }

    [DataContract]
    public partial class Agent
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int OrganizationId { get; set; }
        [DataMember]
        public string Application { get; set; }
        [DataMember]
        public string AgentCode { get; set; }
        [DataMember]
        public string AgentSecret { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string ServiceUrl { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string Notes { get; set; }
        [DataMember]
        public string SystemInfo { get; set; }
    }

    [DataContract]
    public partial class App
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int OrganizationId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public bool Private { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string Assembly { get; set; }
        [DataMember]
        public string AssemblyInfo { get; set; }
        [DataMember]
        public string ReleaseNotes { get; set; }
        [DataMember]
        public string Documentation { get; set; }
        [DataMember]
        public string DocumentationExtension { get; set; }
        [DataMember]
        public string Icon { get; set; }
    }

    [DataContract]
    public partial class Log
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int AgentId { get; set; }
        [DataMember]
        public System.DateTime Date { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public partial class Organization
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Phone { get; set; }
    }

    [DataContract]
    public partial class Property
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ActionId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int PropertyTypeId { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public string DataType { get; set; }
        [DataMember]
        public string Value { get; set; }
    }

    [DataContract]
    public partial class Resource
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ActionId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Format { get; set; }
        [DataMember]
        public string Object { get; set; }
    }

    [DataContract]
    public partial class Schedule
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int AgentId { get; set; }
        [DataMember]
        public int Occurence { get; set; }
        [DataMember]
        public int OccurenceValue { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public System.DateTime? StartDate { get; set; }
        [DataMember]
        public System.DateTime? PollDate { get; set; }
    }

    [DataContract]
    public partial class Setting
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int AgentId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string DataType { get; set; }
    }

    [DataContract]
    public partial class User
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int OrganizationId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Mobile { get; set; }
        [DataMember]
        public int RoleId { get; set; }
        [DataMember]
        public bool Active { get; set; }
    }

    [DataContract]
    public partial class __RefactorLog
    {
        [DataMember]
        public System.Guid OperationKey { get; set; }
    }

}
