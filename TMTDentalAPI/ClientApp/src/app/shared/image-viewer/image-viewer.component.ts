import { Component, OnInit, Input } from '@angular/core';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { environment } from 'src/environments/environment';
import { IrAttachmentBasic } from '../shared';
import { PartnerImageViewModel, PartnerImageBasic } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-image-viewer',
  templateUrl: './image-viewer.component.html',
  styleUrls: ['./image-viewer.component.css']
})
export class ImageViewerComponent implements OnInit {

  partnerImages: PartnerImageBasic[] = [];
  partnerImageSelected: PartnerImageBasic;

  imageApi: string;
  imageDownloadApi: string;

  style = { transform: '', msTransform: '', oTransform: '', webkitTransform: '' };
  scale = 1;
  rotation = 0;
  minScale = 0.6;
  zoomFactor = 0.1;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.imageApi = environment.uploadDomain + 'api/Web/Image';
    this.imageDownloadApi = environment.uploadDomain + 'api/Web/Content';
  }

  stopPropagation(e) {
    e.stopPropagation();
  }
  
  showPrevious() {
    this.resetAll();
    const index = this.partnerImages.findIndex(x => x.uploadId === this.partnerImageSelected.uploadId);
    if (index > 0) {
      this.partnerImageSelected = this.partnerImages[index - 1];
    } else if (index === 0) {
      this.partnerImageSelected = this.partnerImages[this.partnerImages.length - 1];
    }
    // if (model) {
    //   var index = _.findIndex(model.partnerImages, o => o.id == this.partnerImageSelected.id);
    //   if (index > 0) {
    //     this.partnerImageSelected = model.partnerImages[index - 1];
    //   } else {
    //     this.partnerImageSelected = model.partnerImages[model.partnerImages.length - 1];
    //   }
    // }
    return false;
  }

  showNext() {
    this.resetAll();
    const index = this.partnerImages.findIndex(x => x.uploadId === this.partnerImageSelected.uploadId);
    if (index < (this.partnerImages.length - 1)) {
      this.partnerImageSelected = this.partnerImages[index + 1];
    } else {
      this.partnerImageSelected = this.partnerImages[0];
    }
    // var model = this.partnerImages.find(x => x.date == this.partnerImageSelected.date);
    // if (model) {
    //   var index = _.findIndex(model.partnerImages, o => o.id == this.partnerImageSelected.id);
    //   if (index < model.partnerImages.length - 1) {
    //     this.partnerImageSelected = model.partnerImages[index + 1];
    //   } else {
    //     this.partnerImageSelected = model.partnerImages[0];
    //   }
    // }
    return false;
  }

  resetAll() {
    this.scale = 1;
    this.rotation = 0;
    this.updateStyle();
  }

  zoomOut() {
    this.scale -= this.zoomFactor;
    if (this.scale < this.minScale) {
      this.scale = this.minScale;
    }

    this.updateStyle();
    return false;
  }

  zoomIn() {
    this.scale += this.zoomFactor;
    this.updateStyle();
    return false;
  }

  resetZoom() {
    this.scale = 1;
    this.updateStyle();
    return false;
  }

  scrollZoom(evt) {
    evt.deltaY > 0 ? this.zoomOut() : this.zoomIn();
    return false;
  }

  rotate() {
    this.rotation += 90;
    this.updateStyle();
    return false;
  }

  updateStyle() {
    var transform = `scale3d(${this.scale},${this.scale},1) rotate(${this.rotation}deg)`;
    this.style = {
      transform: transform,
      msTransform: transform,
      oTransform: transform,
      webkitTransform: transform
    };
    // this.style.transform = `scale3d(${this.scale},${this.scale},1) rotate(${this.rotation}deg)`;
    // this.style3.transform = `scale3d(${this.scale},${this.scale},1) rotate(${this.rotation}deg)`;
    // this.style.msTransform = this.style.transform;
    // this.style.webkitTransform = this.style.transform;
    // this.style.oTransform = this.style.transform;
    console.log(this.style);
  }

  close() {
    this.activeModal.dismiss();
    return false;
  }
}
