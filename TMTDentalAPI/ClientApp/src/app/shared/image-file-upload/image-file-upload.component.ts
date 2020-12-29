import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { WebService } from 'src/app/core/services/web.service';
import { environment } from '../../../environments/environment';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ImageViewerComponent } from '../image-viewer/image-viewer.component';
import { PartnerWebcamComponent } from '../partner-webcam/partner-webcam.component';

@Component({
  selector: 'app-image-file-upload',
  templateUrl: './image-file-upload.component.html',
  styleUrls: ['./image-file-upload.component.css']
})
export class ImageFileUploadComponent implements OnInit, OnChanges {

  placeholder = '/assets/images/placeholder.png';
  @Input() imageId: string;
  uploading = false;
  @Output() uploaded = new EventEmitter<any>();
  @Input() width: number;
  @Input() height: number;
  @Input() crop: boolean;

  constructor(private webService: WebService, private modalService: NgbModal) { }

  ngOnChanges(changes: SimpleChanges): void {
  }

  ngOnInit() {
  }

  onFileChange(e) {
    var file_node = e.target;
    var file = file_node.files[0];

    var formData = new FormData();
    formData.append('file', file);

    this.webService.uploadImage(formData).subscribe((result: any) => {
      this.uploaded.emit(result);
    });
  }

  onClear() {
    this.uploaded.emit(null);
  }

  get imageFileUrl() {
    if (this.width || this.height) {
      var url = this.imageId + `?width=${this.width}&height=${this.height}`;
      if (this.crop) {
        url = url + '&mode=crop';
      }

      return url;
    }
    
    return this.imageId;
  }

  onWebcam() {
    const modalRef = this.modalService.open(PartnerWebcamComponent, { scrollable: true, size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.result.then(uri => {
      if (uri) {
        this.webService.UploadImageByBase64(uri).subscribe((result: any) => {
          this.uploaded.emit(result);
        });
      }
    }, err => {
      console.log(err);
    });
  }
}
