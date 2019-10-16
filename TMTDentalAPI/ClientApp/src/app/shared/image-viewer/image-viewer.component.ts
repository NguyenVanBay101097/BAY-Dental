import { Component, OnInit, Input } from '@angular/core';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { environment } from 'src/environments/environment';
import { IrAttachmentBasic } from '../shared';

@Component({
  selector: 'app-image-viewer',
  templateUrl: './image-viewer.component.html',
  styleUrls: ['./image-viewer.component.css']
})
export class ImageViewerComponent implements OnInit {

  attachments: IrAttachmentBasic[] = [];
  attachmentSelected: IrAttachmentBasic;

  imageApi: string;
  imageDownloadApi: string;

  style = { transform: '', msTransform: '', oTransform: '', webkitTransform: '' };
  scale = 1;
  rotation = 0;
  minScale = 0.6;
  zoomFactor = 0.1;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.imageApi = environment.apiDomain + 'api/Web/Image';
    this.imageDownloadApi = environment.apiDomain + 'api/Web/Content';
  }

  showPrevious() {
    var index = _.findIndex(this.attachments, o => o.id == this.attachmentSelected.id);
    if (index > 0) {
      this.attachmentSelected = this.attachments[index - 1];
    } else {
      this.attachmentSelected = this.attachments[this.attachments.length - 1];
    }

    return false;
  }

  showNext() {
    this.resetAll();
    var index = _.findIndex(this.attachments, o => o.id == this.attachmentSelected.id);
    if (index < this.attachments.length - 1) {
      this.attachmentSelected = this.attachments[index + 1];
    } else {
      this.attachmentSelected = this.attachments[0];
    }

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
    debugger;
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
