using AutoMapper;
using ContractService.Models;
using ContractService.DTOs;

namespace ContractService.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Contract mappings
            CreateMap<Contract, ContractDto>()
                .ForMember(dest => dest.ContractTypeName, opt => opt.MapFrom(src => src.ContractType != null ? src.ContractType.Name : ""))
                .ForMember(dest => dest.ContractTypeColor, opt => opt.MapFrom(src => src.ContractType != null ? src.ContractType.Color : "#007bff"))
                .ForMember(dest => dest.ContractTypeIcon, opt => opt.MapFrom(src => src.ContractType != null ? src.ContractType.Icon : null))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.GetDescription()))
                .ForMember(dest => dest.StatusColor, opt => opt.MapFrom(src => GetStatusColor(src.Status)))
                .ForMember(dest => dest.PriorityDisplay, opt => opt.MapFrom(src => src.PriorityDisplay))
                .ForMember(dest => dest.FormattedValue, opt => opt.MapFrom(src => src.FormattedValue))
                .ForMember(dest => dest.TagsList, opt => opt.MapFrom(src => src.TagsList))
                .ForMember(dest => dest.DocumentSizeFormatted, opt => opt.MapFrom(src => FormatFileSize(src.DocumentSize)))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired))
                .ForMember(dest => dest.IsExpiringSoon, opt => opt.MapFrom(src => src.IsExpiringSoon))
                .ForMember(dest => dest.DaysUntilExpiration, opt => opt.MapFrom(src => src.DaysUntilExpiration))
                .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => src.DurationDays))
                .ForMember(dest => dest.IsFullySigned, opt => opt.MapFrom(src => src.IsFullySigned))
                .ForMember(dest => dest.PrimaryCounterparty, opt => opt.MapFrom(src => src.PrimaryCounterparty))
                .ForMember(dest => dest.PartiesCount, opt => opt.MapFrom(src => src.Parties.Count))
                .ForMember(dest => dest.SignedPartiesCount, opt => opt.MapFrom(src => src.Parties.Count(p => p.IsSigned)))
                .ForMember(dest => dest.HasChildContracts, opt => opt.MapFrom(src => src.ChildContracts.Any()))
                .ForMember(dest => dest.OwnerName, opt => opt.Ignore()) // Will be populated from service layer
                .ForMember(dest => dest.ApprovedByName, opt => opt.Ignore())
                .ForMember(dest => dest.LastStatusChangedByName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore());

            CreateMap<CreateContractDto, Contract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ContractNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.SignedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedById, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovalNotes, opt => opt.Ignore())
                .ForMember(dest => dest.LastStatusChangeDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastStatusChangedById, opt => opt.Ignore())
                .ForMember(dest => dest.StatusChangeReason, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentPath, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentFileName, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentContentType, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentSize, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ContractType, opt => opt.Ignore())
                .ForMember(dest => dest.Parties, opt => opt.Ignore())
                .ForMember(dest => dest.ParentContract, opt => opt.Ignore())
                .ForMember(dest => dest.ChildContracts, opt => opt.Ignore());

            CreateMap<UpdateContractDto, Contract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ContractNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.SignedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedById, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovalNotes, opt => opt.Ignore())
                .ForMember(dest => dest.LastStatusChangeDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastStatusChangedById, opt => opt.Ignore())
                .ForMember(dest => dest.StatusChangeReason, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentPath, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentFileName, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentContentType, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentSize, opt => opt.Ignore())
                .ForMember(dest => dest.ParentContractId, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ContractType, opt => opt.Ignore())
                .ForMember(dest => dest.Parties, opt => opt.Ignore())
                .ForMember(dest => dest.ParentContract, opt => opt.Ignore())
                .ForMember(dest => dest.ChildContracts, opt => opt.Ignore());

            // Contract Party mappings
            CreateMap<ContractParty, ContractPartyDto>()
                .ForMember(dest => dest.PartyTypeDisplay, opt => opt.MapFrom(src => GetPartyTypeDisplay(src.PartyType)))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.FormattedAddress, opt => opt.MapFrom(src => src.FormattedAddress))
                .ForMember(dest => dest.IsSigned, opt => opt.MapFrom(src => src.IsSigned));

            CreateMap<CreateContractPartyDto, ContractParty>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ContractId, opt => opt.Ignore())
                .ForMember(dest => dest.PartyType, opt => opt.MapFrom(src => (PartyType)src.PartyType))
                .ForMember(dest => dest.SignedDate, opt => opt.Ignore())
                .ForMember(dest => dest.SignedByName, opt => opt.Ignore())
                .ForMember(dest => dest.SignedByTitle, opt => opt.Ignore())
                .ForMember(dest => dest.SignatureData, opt => opt.Ignore())
                .ForMember(dest => dest.SignatureIpAddress, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Contract, opt => opt.Ignore());

            // Contract Type mappings
            CreateMap<ContractType, ContractTypeDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.ContractsCount, opt => opt.MapFrom(src => src.ContractsCount));

            CreateMap<CreateContractTypeDto, ContractType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore());
        }

        private static string GetStatusColor(ContractStatus status)
        {
            return status switch
            {
                ContractStatus.Draft => "#6c757d", // Gray
                ContractStatus.UnderReview => "#fd7e14", // Orange
                ContractStatus.PendingApproval => "#ffc107", // Yellow
                ContractStatus.Approved => "#20c997", // Teal
                ContractStatus.Active => "#28a745", // Green
                ContractStatus.Suspended => "#6f42c1", // Purple
                ContractStatus.Expired => "#dc3545", // Red
                ContractStatus.Terminated => "#dc3545", // Red
                ContractStatus.Renewed => "#17a2b8", // Cyan
                ContractStatus.Cancelled => "#6c757d", // Gray
                _ => "#6c757d"
            };
        }

        private static string GetPartyTypeDisplay(PartyType partyType)
        {
            return partyType switch
            {
                PartyType.Internal => "Internal",
                PartyType.External => "External",
                PartyType.Witness => "Witness",
                PartyType.LegalRepresentative => "Legal Representative",
                _ => partyType.ToString()
            };
        }

        private static string? FormatFileSize(long? sizeInBytes)
        {
            if (!sizeInBytes.HasValue) return null;
            
            var size = sizeInBytes.Value;
            string[] suf = { " B", " KB", " MB", " GB", " TB" };
            if (size == 0) return "0" + suf[0];
            
            var bytes = Math.Abs(size);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(size) * num) + suf[place];
        }
    }
}