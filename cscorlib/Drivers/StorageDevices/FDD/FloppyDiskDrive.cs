using System;

namespace CooS.Drivers.StorageDevices.FDD {

	public class FloppyDiskDrive {

		const int FD_BYTES_PER_SECTOR		= 512;
		const int FD_SECTORS_PER_HEAD		= 18;
		const int FD_HEADS_PER_CYLINDER		= 2;
		const int FD_CYLINDERS_PER_MEDIA	= 80;
		const int FD_SECTORS_PER_CYLINDER	= FD_SECTORS_PER_HEAD*FD_HEADS_PER_CYLINDER;

		FloppyDiskController fdc;
		int drive_no;
		uint current_cylinder;
		uint current_head;
		uint current_sector;

		public FloppyDiskDrive(FloppyDiskController fdc, int driveno) {
			this.fdc = fdc;
			this.drive_no = driveno;
			this.current_cylinder = 0;
			this.current_head = 0;
			this.current_sector = 0;
		}

		public void Seek(uint blkno) {
			uint cylinder = blkno/FD_SECTORS_PER_CYLINDER;
			if(cylinder!=current_cylinder) {
				lock(fdc) {
					fdc.SelectDrive(drive_no);
					fdc.Seek(cylinder);
				}
				current_cylinder = cylinder;
			}
			uint trackseq = blkno % FD_SECTORS_PER_CYLINDER;
			current_head	= trackseq/FD_SECTORS_PER_HEAD;
			current_sector	= trackseq%FD_SECTORS_PER_HEAD;
		}

		public void Read(byte[] buf, int index, uint count) {
			fdc.SelectDrive(drive_no);
			ReadDataResult result = fdc.ReadData(buf, index, drive_no, (byte)current_cylinder, (byte)current_head, (byte)current_sector, count);
			current_cylinder	= result.c;
			current_head		= result.h;
			current_sector		= result.r;
		}

	}
	
}
