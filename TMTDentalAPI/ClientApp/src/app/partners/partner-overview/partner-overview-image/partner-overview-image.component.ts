import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { WebService } from 'src/app/core/services/web.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { IrAttachmentService } from 'src/app/shared/ir-attachment.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { IrAttachmentBasic } from 'src/app/shared/shared';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { environment } from 'src/environments/environment';
import { PartnerImageBasic, PartnerImageViewModel, PartnerService } from '../../partner.service';

@Component({
  selector: 'app-partner-overview-image',
  templateUrl: './partner-overview-image.component.html',
  styleUrls: ['./partner-overview-image.component.css']
})
export class PartnerOverviewImageComponent implements OnInit {

  webImageApi: string;
  webContentApi: string;
  imagesPreview: IrAttachmentBasic[] = [];
  imageViewModels: PartnerImageViewModel[] = [];
  @Input() partnerId: string;
  formGroup: FormGroup;
  imagesGroup: any = [];
  constructor(
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private fb: FormBuilder,
    private intlService: IntlService,
    private activeRoute: ActivatedRoute,
    private showErrorService: AppSharedShowErrorService,
    private webService: WebService,
    private irAttachmentService: IrAttachmentService,
    private notifyService: NotifyService
  ) { }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');

    this.formGroup = this.fb.group({
      date: new Date(),
      note: ''
    });
    this.webImageApi = environment.uploadDomain + 'api/Web/Image';
    this.webContentApi = environment.uploadDomain + 'api/Web/Content';
    if (this.partnerId) {
      this.loadData();
    }
  }

  stopPropagation(e) {
    e.stopPropagation();
  }

  addPartnerImages(e) {
    var file_node = e.target;
    var count = file_node.files.length;
    var formData = new FormData();
    formData.append('id', this.partnerId);
    formData.append('model', "partner");
    for (let i = 0; i < count; i++) {
      var file = file_node.files[i];
      formData.append('files', file);
      var filereader = new FileReader();
      filereader.readAsDataURL(file);
    }
    this.webService.binaryUploadAttachment(formData).subscribe((res: any) => {
      this.loadData();
    })
  }

  deletePartnerImages(index, event) {
    var item = this.imagesPreview[index];
    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa hình ảnh ' + item.name;
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa hình ảnh?';

    modalRef.result.then(() => {
      this.irAttachmentService.deleteImage(item.id).subscribe(
        () => {
          // this.loadData();
          this.notifyService.notify('success', 'Xóa thành công');
          this.imagesPreview.splice(index, 1);
        })
    })
  }

  loadData() {
    this.imagesPreview = [];
    this.partnerService.getListAttachment(this.partnerId).subscribe(
      result => {
        if (result) {
          this.imagesPreview = result;
        }
      }
    )
  }

  viewImage(partnerImage: IrAttachmentBasic) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.images = this.imagesPreview;
    modalRef.componentInstance.selectedImage = partnerImage;
  }

}