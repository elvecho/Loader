using Loader.Models;
using Microsoft.EntityFrameworkCore;

namespace Loader.Persistence
{
    public class RT_ChecksContext : DbContext
    {

        public RT_ChecksContext(DbContextOptions<RT_ChecksContext> options)
            : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.LDocumentTypeId);

                entity.ToTable("Document_Type", "LOOKUP");

                entity.Property(e => e.LDocumentTypeId)
                    .HasColumnName("lDocumentTypeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .HasColumnName("szDescription")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<IndexMaintenance>(entity =>
            {
                entity.HasKey(e => new { e.DDate, e.SzDbName, e.SzSchemaName, e.SzTableName, e.SzIndexName });

                entity.ToTable("IndexMaintenance", "LOGGING");

                entity.Property(e => e.DDate)
                    .HasColumnName("dDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDbName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SzSchemaName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzTableName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SzIndexName)
                    .HasColumnName("szIndexName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.LPostPercFragmentation).HasColumnName("lPostPercFragmentation");

                entity.Property(e => e.LPrePercFragmentation).HasColumnName("lPrePercFragmentation");

                entity.Property(e => e.SzActionToDo)
                    .HasColumnName("szActionToDo")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RtServer>(entity =>
            {
                entity.HasKey(e => new { e.SzRtServerId, e.LRetailStoreId, e.LStoreGroupId });

                entity.ToTable("RtServer", "LOOKUP");

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.BOnDutyFlag).HasColumnName("bOnDutyFlag");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzIpAddress)
                    .HasColumnName("szIpAddress")
                    .HasMaxLength(100);

                entity.Property(e => e.SzLocation)
                    .HasColumnName("szLocation")
                    .HasMaxLength(100);

                entity.Property(e => e.SzPassword)
                    .HasColumnName("szPassword")
                    .HasMaxLength(100);

                entity.Property(e => e.SzUsername)
                    .HasColumnName("szUsername")
                    .HasMaxLength(100);

                entity.HasOne(d => d.L)
                    .WithMany(p => p.RtServer)
                    .HasForeignKey(d => new { d.LRetailStoreId, d.LStoreGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RtServer_#1");
            });

            modelBuilder.Entity<RtServerStatus>(entity =>
            {
                entity.HasKey(e => new { e.SzRtServerId, e.LRetailStoreId, e.LStoreGroupId })
                    .HasName("PK_ServerRt_Status");

                entity.ToTable("RtServer_Status", "LOOKUP");

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.BOnErrorFlag).HasColumnName("bOnErrorFlag");

                entity.Property(e => e.BRunningTransmissionFlag).HasColumnName("bRunningTransmissionFlag");

                entity.Property(e => e.BVatVentilationFlag).HasColumnName("bVatVentilationFlag");

                entity.Property(e => e.DGrandTotalAmount)
                    .HasColumnName("dGrandTotalAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DLastDateTimeTransactionsCollected)
                    .HasColumnName("dLastDateTimeTransactionsCollected")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastDateTimeTransmissionsCollected)
                    .HasColumnName("dLastDateTimeTransmissionsCollected")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastDateTimeCollected)
                    .HasColumnName("dLastDateTimeCollected")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastDateTimeRead)
                    .HasColumnName("dLastDateTimeRead")
                    .HasColumnType("datetime");

                entity.Property(e => e.LLastClosureNmbr).HasColumnName("lLastClosureNmbr");

                entity.Property(e => e.LMemoryAvailable).HasColumnName("lMemoryAvailable");

                entity.Property(e => e.LPendingTransmissionDays).HasColumnName("lPendingTransmissionDays");

                entity.Property(e => e.LPendingTransmissionNmbr).HasColumnName("lPendingTransmissionNmbr");

                entity.Property(e => e.LTransmissionScheduleHoursRepeat).HasColumnName("lTransmissionScheduleHoursRepeat");

                entity.Property(e => e.LTransmissionScheduleMinutesLeft).HasColumnName("lTransmissionScheduleMinutesLeft");

                entity.Property(e => e.SzErrorDescription)
                    .HasColumnName("szErrorDescription")
                    .HasMaxLength(255);

                entity.Property(e => e.SzLastCloseResult)
                    .HasColumnName("szLastCloseResult")
                    .HasMaxLength(255);

                entity.HasOne(d => d.RtServer)
                    .WithOne(p => p.RtServerStatus)
                    .HasForeignKey<RtServerStatus>(d => new { d.SzRtServerId, d.LRetailStoreId, d.LStoreGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RtServer_Status_#1");
            });

            modelBuilder.Entity<RtServerTransmission>(entity =>
            {
                entity.HasKey(e => new { e.SzRtServerId, e.LRtServerOperationId })
                    .HasName("PK_ServerRt_Transmission");

                entity.ToTable("RtServer_Transmission", "TRX");

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<RtServerTransmissionDetail>(entity =>
            {
                entity.HasKey(e => new { e.SzRtServerId, e.LRtServerOperationId, e.LRtDeviceTransmissionId, e.SzRtDeviceId });

                entity.ToTable("RtServer_TransmissionDetail", "TRX");

                entity.Property(e => e.SzRtServerId)
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.LRtDeviceTransmissionId).HasColumnName("lRtDeviceTransmissionID");

                entity.Property(e => e.SzRtDeviceId)
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtDeviceClosureDateTime)
                    .HasColumnName("dRtDeviceClosureDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtInactivityDateTimeFrom)
                    .HasColumnName("dRtInactivityDateTimeFrom")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtInactivityDateTimeTo)
                    .HasColumnName("dRtInactivityDateTimeTo")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzRtDeviceType)
                    .HasColumnName("szRtDeviceType")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.SzRtTransmissionFormat)
                    .HasColumnName("szRtTransmissionFormat")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.RtServerTransmission)
                    .WithMany(p => p.RtServerTransmissionDetail)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRtServerOperationId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServerRt_TransmissionDetail_#1");
            });

            modelBuilder.Entity<RtServerTransmissionDetailRtData>(entity =>
            {
                entity.HasKey(e => e.LRtDataId)
                    .HasName("PK_ServerRT_TransmissionDetail_RtData");

                entity.ToTable("RtServer_TransmissionDetail_RtData", "TRX");

                entity.Property(e => e.LRtDataId).HasColumnName("lRtDataID");

                entity.Property(e => e.BVatVentilation).HasColumnName("bVatVentilation");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DReturnAmount)
                    .HasColumnName("dReturnAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DSaleAmount)
                    .HasColumnName("dSaleAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DVatAmount)
                    .HasColumnName("dVatAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DVatRate)
                    .HasColumnName("dVatRate")
                    .HasColumnType("decimal(6, 4)");

                entity.Property(e => e.DVoidAmount)
                    .HasColumnName("dVoidAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.LRtDeviceTransmissionId).HasColumnName("lRtDeviceTransmissionID");

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.SzRtDeviceId)
                    .IsRequired()
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .IsRequired()
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzVatLegalReference)
                    .HasColumnName("szVatLegalReference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SzVatNature)
                    .HasColumnName("szVatNature")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.RtServerTransmissionDetail)
                    .WithMany(p => p.RtServerTransmissionDetailRtData)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRtServerOperationId, d.LRtDeviceTransmissionId, d.SzRtDeviceId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServerRT_TransmissionDetail_RtData_#1");
            });

            modelBuilder.Entity<RtServerTransmissionDetailRtReport>(entity =>
            {
                entity.HasKey(e => e.LRtReportId)
                    .HasName("PK_ServerRt_Report");

                entity.ToTable("RtServer_TransmissionDetail_RtReport", "TRX");

                entity.Property(e => e.LRtReportId).HasColumnName("lRtReportID");

                entity.Property(e => e.DEventDateTime)
                    .HasColumnName("dEventDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.LRtDeviceTransmissionId).HasColumnName("lRtDeviceTransmissionID");

                entity.Property(e => e.LRtServerOperationId).HasColumnName("lRtServerOperationID");

                entity.Property(e => e.SzEventNote)
                    .HasColumnName("szEventNote")
                    .HasMaxLength(255);

                entity.Property(e => e.SzEventType)
                    .HasColumnName("szEventType")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.SzRtDeviceId)
                    .IsRequired()
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .IsRequired()
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.RtServerTransmissionDetail)
                    .WithMany(p => p.RtServerTransmissionDetailRtReport)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRtServerOperationId, d.LRtDeviceTransmissionId, d.SzRtDeviceId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServerRt_TransmissionDetail_RtReport_#1");
            });

            modelBuilder.Entity<RtServerVat>(entity =>
            {
                entity.HasKey(e => e.SzVatCodeId);

                entity.ToTable("RtServer_VAT", "LOOKUP");

                entity.Property(e => e.SzVatCodeId)
                    .HasColumnName("szVatCodeID")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .HasColumnName("szDescription")
                    .HasMaxLength(100);

                entity.Property(e => e.SzVatNature)
                    .IsRequired()
                    .HasColumnName("szVatNature")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => new { e.LRetailStoreId, e.LStoreGroupId });

                entity.ToTable("Store", "LOOKUP");

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .IsRequired()
                    .HasColumnName("szDescription")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.LStoreGroup)
                    .WithMany(p => p.Store)
                    .HasForeignKey(d => d.LStoreGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_#1");
            });

            modelBuilder.Entity<StoreGroup>(entity =>
            {
                entity.HasKey(e => e.LStoreGroupId)
                    .HasName("PK_StoreGroup");

                entity.ToTable("Store_Group", "LOOKUP");

                entity.Property(e => e.LStoreGroupId)
                    .HasColumnName("lStoreGroupID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .IsRequired()
                    .HasColumnName("szDescription")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<TransactionAffiliation>(entity =>
            {
                entity.HasKey(e => e.SzRtDocumentId);

                entity.ToTable("Transaction_Affiliation", "TRN");

                entity.Property(e => e.SzRtDocumentId)
                    .HasColumnName("szRtDocumentID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.BRtNonCompliantFlag).HasColumnName("bRtNonCompliantFlag");

                entity.Property(e => e.BTransactionArchivedFlag).HasColumnName("bTransactionArchivedFlag");

                entity.Property(e => e.BTransactionCheckedFlag).HasColumnName("bTransactionCheckedFlag");

                entity.Property(e => e.DBusinessDate)
                    .HasColumnName("dBusinessDate")
                    .HasColumnType("date");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DPosDateTime)
                    .HasColumnName("dPosDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DPosTransactionTurnover)
                    .HasColumnName("dPosTransactionTurnover")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DRtDateTime)
                    .HasColumnName("dRtDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtTransactionTurnover)
                    .HasColumnName("dRtTransactionTurnover")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.LPosTaNmbr).HasColumnName("lPosTaNmbr");

                entity.Property(e => e.LPosReceivedTransactionCounter).HasColumnName("lPosReceivedTransactionCounter");

                entity.Property(e => e.LRtReceivedTransactionCounter).HasColumnName("lRtReceivedTransactionCounter");

                entity.Property(e => e.LPosWorkstationNmbr).HasColumnName("lPosWorkstationNmbr");

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LRtClosureNmbr).HasColumnName("lRtClosureNmbr");

                entity.Property(e => e.LRtDocumentNmbr).HasColumnName("lRtDocumentNmbr");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.LTransactionMismatchId).HasColumnName("lTransactionMismatchID");

                entity.Property(e => e.SzRtDeviceId)
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .IsRequired()
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzTranscationCheckNote)
                    .HasColumnName("szTranscationCheckNote")
                    .HasMaxLength(255);

                entity.HasOne(d => d.LTransactionMismatch)
                    .WithMany(p => p.TransactionAffiliation)
                    .HasForeignKey(d => d.LTransactionMismatchId)
                    .HasConstraintName("FK_Transaction_Affiliation_#2");

                entity.HasOne(d => d.RtServer)
                    .WithMany(p => p.TransactionAffiliation)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRetailStoreId, d.LStoreGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Affiliation_#1");
            });

            modelBuilder.Entity<TransactionDocument>(entity =>
            {
                entity.HasKey(e => e.SzRtDocumentId);

                entity.ToTable("Transaction_Document", "TRN");

                entity.Property(e => e.LTransactionDocumentId).HasColumnName("lTransactionDocumentID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.LDocumentTypeId).HasColumnName("lDocumentTypeID");

                entity.Property(e => e.SzRtDocumentId)
                    .HasColumnName("szRtDocumentID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SzDocumentAttachment)
                    .HasColumnName("szDocumentAttachment")
                    .HasColumnType("xml");

                entity.Property(e => e.SzDocumentAttachmentTxt).HasColumnName("szDocumentAttachmentTxt");

                entity.Property(e => e.SzDocumentName)
                    .HasColumnName("szDocumentName")
                    .HasMaxLength(100);

                entity.Property(e => e.SzDocumentNote)
                    .HasColumnName("szDocumentNote")
                    .HasMaxLength(255);

                entity.HasOne(d => d.LDocumentType)
                    .WithMany(p => p.TransactionDocument)
                    .HasForeignKey(d => d.LDocumentTypeId)
                    .HasConstraintName("FK_Transaction_Document_#1");

                entity.HasOne(d => d.LTransactionAffiliation)
                    .WithMany(p => p.TransactionDocument)
                    .HasForeignKey(d => d.SzRtDocumentId)
                    .HasConstraintName("FK_Transaction_Document_#2");
            });

            modelBuilder.Entity<TransactionMismatch>(entity =>
            {
                entity.HasKey(e => e.LTransactionMismatchId);

                entity.ToTable("Transaction_Mismatch", "LOOKUP");

                entity.Property(e => e.LTransactionMismatchId)
                    .HasColumnName("lTransactionMismatchID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.SzDescription)
                    .HasColumnName("szDescription")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<TransactionRtError>(entity =>
            {
                entity.HasKey(e => e.LRtErrorId);

                entity.ToTable("Transaction_RtError", "TRN");

                entity.Property(e => e.LRtErrorId).HasColumnName("lRtErrorID");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DRtDateTime)
                    .HasColumnName("dRtDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LRtClosureNmbr).HasColumnName("lRtClosureNmbr");

                entity.Property(e => e.LRtDocumentNmbr).HasColumnName("lRtDocumentNmbr");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.SzDescription)
                    .HasColumnName("szDescription")
                    .HasMaxLength(255);

                entity.Property(e => e.SzRtDeviceId)
                    .HasColumnName("szRtDeviceID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SzRtServerId)
                    .IsRequired()
                    .HasColumnName("szRtServerID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.RtServer)
                    .WithMany(p => p.TransactionRtError)
                    .HasForeignKey(d => new { d.SzRtServerId, d.LRetailStoreId, d.LStoreGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_RtError_#1");
            });

            modelBuilder.Entity<TransactionVat>(entity =>
            {
                entity.HasKey(e => new { e.SzRtDocumentId, e.SzVatCodeId });

                entity.ToTable("Transaction_Vat", "TRN");

                entity.Property(e => e.SzRtDocumentId)
                    .HasColumnName("szRtDocumentID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SzVatCodeId)
                    .HasColumnName("szVatCodeID")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.BVatCheckedFlag).HasColumnName("bVatCheckedFlag");

                entity.Property(e => e.BVatMismatchFlag).HasColumnName("bVatMismatchFlag");

                entity.Property(e => e.DLastUpdateLocal)
                    .HasColumnName("dLastUpdateLocal")
                    .HasColumnType("datetime");

                entity.Property(e => e.DPosGrossAmount)
                    .HasColumnName("dPosGrossAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DPosNetAmount)
                    .HasColumnName("dPosNetAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DPosVatAmount)
                    .HasColumnName("dPosVatAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DPosVatRate)
                    .HasColumnName("dPosVatRate")
                    .HasColumnType("decimal(6, 4)");

                entity.Property(e => e.DRtGrossAmount)
                    .HasColumnName("dRtGrossAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DRtNetAmount)
                    .HasColumnName("dRtNetAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DRtVatAmount)
                    .HasColumnName("dRtVatAmount")
                    .HasColumnType("decimal(15, 4)");

                entity.Property(e => e.DRtVatRate)
                    .HasColumnName("dRtVatRate")
                    .HasColumnType("decimal(6, 4)");

                entity.HasOne(d => d.LTransactionAffiliation)
                    .WithMany(p => p.TransactionVat)
                    .HasForeignKey(d => d.SzRtDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Vat_#1");

                entity.HasOne(d => d.SzVatCode)
                    .WithMany(p => p.TransactionVat)
                    .HasForeignKey(d => d.SzVatCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Vat_#2");
            });

            modelBuilder.Entity<Xmltransactions>(entity =>
            {
                entity.ToTable("XMLTransactions", "LOAD");

                entity.Property(e => e.BodyTransavtion)
                    .IsRequired()
                    .HasColumnType("xml");

                entity.Property(e => e.DtBusinessDate)
                    .HasColumnName("dtBusinessDate")
                    .HasColumnType("date");

                entity.Property(e => e.LPosTaNmbr).HasColumnName("lPosTaNmbr");

                entity.Property(e => e.LPosWorkstationNmbr).HasColumnName("lPosWorkstationNmbr");

                entity.Property(e => e.LRetailStoreId).HasColumnName("lRetailStoreID");

                entity.Property(e => e.LStoreGroupId).HasColumnName("lStoreGroupID");

                entity.Property(e => e.LoadedDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<XmlwithOpenXml>(entity =>
            {
                entity.ToTable("XMLwithOpenXML", "LOAD");

                entity.Property(e => e.LoadedDateTime).HasColumnType("datetime");

                entity.Property(e => e.Xmldata)
                    .IsRequired()
                    .HasColumnName("XMLData")
                    .HasColumnType("xml");
            });

            //OnModelCreatingPartial(modelBuilder);
        }

        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<IndexMaintenance> IndexMaintenance { get; set; }
        public virtual DbSet<RtServer> RtServer { get; set; }
        public virtual DbSet<RtServerStatus> RtServerStatus { get; set; }
        public virtual DbSet<RtServerTransmission> RtServerTransmission { get; set; }
        public virtual DbSet<RtServerTransmissionDetail> RtServerTransmissionDetail { get; set; }
        public virtual DbSet<RtServerTransmissionDetailRtData> RtServerTransmissionDetailRtData { get; set; }
        public virtual DbSet<RtServerTransmissionDetailRtReport> RtServerTransmissionDetailRtReport { get; set; }
        public virtual DbSet<RtServerVat> RtServerVat { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<StoreGroup> StoreGroup { get; set; }
        public virtual DbSet<TransactionAffiliation> TransactionAffiliation { get; set; }
        public virtual DbSet<TransactionDocument> TransactionDocument { get; set; }
        public virtual DbSet<TransactionMismatch> TransactionMismatch { get; set; }
        public virtual DbSet<TransactionRtError> TransactionRtError { get; set; }
        public virtual DbSet<TransactionVat> TransactionVat { get; set; }
        public virtual DbSet<Xmltransactions> Xmltransactions { get; set; }
        public virtual DbSet<XmlwithOpenXml> XmlwithOpenXml { get; set; }
    }
}
