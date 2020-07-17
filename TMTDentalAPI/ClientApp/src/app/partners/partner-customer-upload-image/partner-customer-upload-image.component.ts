import { Component, OnInit } from '@angular/core';
import { PartnerImageBasic, PartnerService, PartnerImageSave, PartnerImageViewModel } from '../partner.service';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { environment } from 'src/environments/environment';
import { DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ActivatedRoute } from '@angular/router';
import { mergeMap, groupBy, reduce } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-partner-customer-upload-image',
  templateUrl: './partner-customer-upload-image.component.html',
  styleUrls: ['./partner-customer-upload-image.component.css']
})
export class PartnerCustomerUploadImageComponent implements OnInit {
  webImageApi: string;
  webContentApi: string;
  imagesPreview: PartnerImageBasic[] = [];
  imageViewModels: PartnerImageViewModel[] = [];
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
      () => {
        this.getImageIds();
      })

  }

  deletePartnerImages(item, event) {
    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa hình ảnh ' + item.name;

    modalRef.result.then(() => {
      this.partnerService.deleteParnerImage(item.id).subscribe(
        () => {
          var imageViewModel = this.imageViewModels.find(x => x.date == item.date);
          if (imageViewModel) {
            var index = imageViewModel.partnerImages.findIndex(x => x.id == item.id);
            imageViewModel.partnerImages.splice(index, 1);
          } else {
            this.getImageIds();
          }
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
          this.imageViewModels = [];
          result.forEach(item => {
            var obj = new PartnerImageViewModel();
            if (!this.imageViewModels.some(x => x.date == item.date)) {
              obj.date = item.date;
              if (!obj.partnerImages) {
                obj.partnerImages = [];
              }
              obj.partnerImages = result.filter(x => x.date == item.date);
              this.imageViewModels.push(obj)
            }
          });
        }
      }
    )
  }

  viewImage(partnerImage: PartnerImageBasic) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.partnerImages = this.imageViewModels;
    modalRef.componentInstance.partnerImageSelected = partnerImage;
  }

}
