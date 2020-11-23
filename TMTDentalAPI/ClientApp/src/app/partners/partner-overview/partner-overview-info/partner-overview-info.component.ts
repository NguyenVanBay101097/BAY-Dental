import { ChangeDetectionStrategy, Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnersService } from 'src/app/shared/services/partners.service';
import { PartnerDisplay } from '../../partner-simple';
import { Pipe, PipeTransform } from '@angular/core';
import { PartnerService } from '../../partner.service';

@Component({
  selector: 'app-partner-overview-info',
  templateUrl: './partner-overview-info.component.html',
  styleUrls: ['./partner-overview-info.component.css']
})
export class PartnerOverviewInfoComponent implements OnInit {
  @Input() partner: any;
  
  constructor(
    private modalService: NgbModal,
    private PartnerOdataService: PartnersService,
    private partnerService : PartnerService
  ) { }

  ngOnInit() {
    console.log(this.partner);
    
  }

  onAvatarUploaded(data: any) {
    this.partner.avatar = data ? data.fileUrl : null;
    this.partnerService.saveAvatar({ partnerId: this.partner.Id, imageId: data ? data.fileUrl : null }).subscribe(() => {
      this.GetPartner(this.partner.Id);
    });
  }

  editCustomer() {
    if (this.partner && this.partner.Id) {
      const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.id = this.partner.Id;
      modalRef.componentInstance.title = 'Cập nhật khách hàng';

      modalRef.result.then(result => {
        this.GetPartner(this.partner.Id);
      }, () => {})
    }
  }

  GetPartner(id) {
    this.PartnerOdataService.getDisplay(id).subscribe((res: any) => {
      this.partner = res;
    });
  }

}
