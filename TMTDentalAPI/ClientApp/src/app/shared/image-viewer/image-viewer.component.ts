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

  images: IrAttachmentBasic[] = [];
  selectedImage: IrAttachmentBasic;

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
    const index = this.images.findIndex(x => x.url === this.selectedImage.url);
    if (index > 0) {
      this.selectedImage = this.images[index - 1];
    } else if (index === 0) {
      this.selectedImage = this.images[this.images.length - 1];
    }
    // if (model) {
    //   var index = _.findIndex(model.images, o => o.id == this.selectedImage.id);
    //   if (index > 0) {
    //     this.selectedImage = model.images[index - 1];
    //   } else {
    //     this.selectedImage = model.images[model.images.length - 1];
    //   }
    // }
    return false;
  }

  showNext() {
    this.resetAll();
    const index = this.images.findIndex(x => x.url === this.selectedImage.url);
    if (index < (this.images.length - 1)) {
      this.selectedImage = this.images[index + 1];
    } else {
      this.selectedImage = this.images[0];
    }
    // var model = this.images.find(x => x.date == this.selectedImage.date);
    // if (model) {
    //   var index = _.findIndex(model.images, o => o.id == this.selectedImage.id);
    //   if (index < model.images.length - 1) {
    //     this.selectedImage = model.images[index + 1];
    //   } else {
    //     this.selectedImage = model.images[0];
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
  }

  close() {
    this.activeModal.dismiss();
    return false;
  }
}
