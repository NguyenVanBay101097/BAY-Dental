import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnersService } from 'src/app/shared/services/partners.service';
import { PartnerDisplay } from '../../partner-simple';
import { Pipe, PipeTransform } from '@angular/core';
import { PartnerService } from '../../partner.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';

@Component({
  selector: 'app-partner-overview-info',
  templateUrl: './partner-overview-info.component.html',
  styleUrls: ['./partner-overview-info.component.css']
})
export class PartnerOverviewInfoComponent implements OnInit {
  @Input() partner: any;
  categories: any[] = [];
  @Output() updateEvent = new EventEmitter<any>();
  showInfo = false;

  constructor(
    private modalService: NgbModal,
    private PartnerOdataService: PartnersService,
    private partnerService: PartnerService,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.categories = this.partner.categories
    this.checkPermission()
  }

  onAvatarUploaded(data: any) {
    this.partner.avatar = data ? data.fileUrl : null;
    this.partnerService.saveAvatar({ partnerId: this.partner.id, imageId: data ? data.fileUrl : null }).subscribe(() => {
      this.GetPartner(this.partner.id);
    });
  }

  editCustomer() {
    if (this.partner && this.partner.id) {
      const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.id = this.partner.id;
      modalRef.componentInstance.title = 'Sửa khách hàng';

      modalRef.result.then(() => {
        this.updateEvent.emit(true);
        // this.GetPartner(this.partner.Id);
      }, () => { })
    }
  }

  GetPartner(id) {
    // this.PartnerOdataService.getDisplay(id).subscribe((res: any) => {
    //   this.partner = res;
    // });
    this.partnerService.getPartner(id).subscribe((result) => {
      this.partner = result;
    });
  }

  checkPermission() {
    this.showInfo = this.checkPermissionService.check(["Basic.Partner.ContactInfo"]);
  }

}
