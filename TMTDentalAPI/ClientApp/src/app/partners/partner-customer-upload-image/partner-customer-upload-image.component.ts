import { Component, OnInit } from '@angular/core';
import { PartnerImageBasic, PartnerService, PartnerImageSave } from '../partner.service';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { environment } from 'src/environments/environment';
import { DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-partner-customer-upload-image',
  templateUrl: './partner-customer-upload-image.component.html',
  styleUrls: ['./partner-customer-upload-image.component.css']
})
export class PartnerCustomerUploadImageComponent implements OnInit {
  webImageApi: string;
  webContentApi: string;
  imagesPreview: PartnerImageBasic[] = [];
  id: string;
  formGroup: FormGroup;
  constructor(
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private fb: FormBuilder,
    private intl: IntlService,
    private activeRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');

    this.formGroup = this.fb.group({
      date: new Date(),
      note: ''
    });
    this.webImageApi = environment.uploadDomain + 'api/Web/Image';
    this.webContentApi = environment.uploadDomain + 'api/Web/Content';
    if (this.id)
      this.getImageIds();
  }

  addPartnerImages(e) {
    var file_node = e.target;
    var count = file_node.files.length;
    var formData = new FormData();
    formData.append('date', this.intl.formatDate(this.formGroup.get('date') ? this.formGroup.get('date').value : null, 'yyyy-MM-dd'));
    formData.append('note', this.formGroup.get('note') ? this.formGroup.get('note').value : 'will be have');
    formData.append('partnerId', this.id);
    for (let i = 0; i < count; i++) {
      var file = file_node.files[i];
      formData.append('files', file);
      var filereader = new FileReader();
      filereader.readAsDataURL(file);
    }
    this.partnerService.uploadPartnerImage(formData).subscribe(
      rs => {
        // this.getImageIds();
        rs.forEach(e => {
          this.imagesPreview.push(e);
        })
      })

  }

  deletePartnerImages(item, event) {
    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa hình ảnh ' + item.name;

    modalRef.result.then(() => {
      this.partnerService.deleteParnerImage(item.id).subscribe(
        () => {
          var index = this.imagesPreview.findIndex(x => x.id == item.id);
          this.imagesPreview.splice(index, 1);
        })
    })
  }

  getImageIds() {
    this.imagesPreview = [];
    var value = {
      partnerId: this.id
    }
    this.partnerService.getPartnerImageIds(value).subscribe(
      result => {
        if (result) {
          result.forEach(item => {
            this.imagesPreview.push(item);
          });
        }
      }
    )
  }

  viewImage(attachment: PartnerImageBasic) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.attachments = this.imagesPreview;
    modalRef.componentInstance.attachmentSelected = attachment;
  }

}
