import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
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
  imagesPreview: PartnerImageBasic[] = [];
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
    private showErrorService: AppSharedShowErrorService
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
      this.getImageIds();
    }
  }

  addPartnerImages(e) {
    var file_node = e.target;
    var count = file_node.files.length;
    var formData = new FormData();
    formData.append('partnerId', this.partnerId);
    for (let i = 0; i < count; i++) {
      var file = file_node.files[i];
      formData.append('files', file);
      var filereader = new FileReader();
      filereader.readAsDataURL(file);
    }
    this.partnerService.uploadPartnerImage(formData).subscribe(() => {
      this.getImageIds();
    }, (err) => {
      this.showErrorService.show(err);
    });

  }

  deletePartnerImages(index, event) {
    var item = this.imagesPreview[index];
    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa hình ảnh ' + item.name;

    modalRef.result.then(() => {
      this.partnerService.deleteParnerImage(item.id).subscribe(
        () => {
          this.imagesPreview.splice(index, 1);
          this.processGroupImages();
        })
    })
  }

  getImageIds() {
    this.imagesPreview = [];
    var value = {
      partnerId: this.partnerId
    }

    this.partnerService.getPartnerImageIds(value).subscribe(
      result => {
        if (result) {
          this.imagesPreview = result;
          this.processGroupImages();
        }
      }
    )
  }

  processGroupImages() {
    var self = this;
    var groups = _.groupBy(this.imagesPreview, function (obj) {
      var date = new Date(obj.date);
      return self.intlService.formatDate(date, 'dd/MM/yyyy');
    });

    this.imagesGroup = _.map(groups, function (group, day) {
      return {
        date: day,
        images: group
      }
    });
  }

  viewImage(partnerImage: PartnerImageBasic) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.partnerImages = this.imageViewModels;
    modalRef.componentInstance.partnerImageSelected = partnerImage;
  }

}