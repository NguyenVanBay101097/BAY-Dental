import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { WebService } from 'src/app/core/services/web.service';
declare var Webcam: any;
@Component({
  selector: 'app-partner-webcam',
  templateUrl: './partner-webcam.component.html',
  styleUrls: ['./partner-webcam.component.css']
})
export class PartnerWebcamComponent implements OnInit {

  image64: any;

  constructor(public activeModal: NgbActiveModal, private webService: WebService, public modalService: NgbModal) { }

  ngOnInit() {
  }

  public ngAfterViewInit() {
    Webcam.set({
      image_format: 'jpeg',
      jpeg_quality: 90
    });
    Webcam.attach('#my_camera');
  }

  onSnap() {
    Webcam.snap(function (data_uri) {
      this.image64 = data_uri;
      document.getElementById('results').innerHTML =
        '<a href="' + this.image64 + '" download="cbimage.jpg"><img style="width: 100%; height: 100%;" src="' + this.image64 + '"/></a>';
    });
  }

  onSave() {
    if (!this.image64) {
      this.activeModal.close();
      return;
    }
    Webcam.reset();
    
    const modalRef = this.modalService.open(PartnerWebcamComponent, { scrollable: true, size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.result.then(uri => {
      if (uri) {
        this.webService.UploadImageByBase64(uri).subscribe((result: any) => {
          this.activeModal.close(result);
        });
      }
    }, err => {
      console.log(err);
    });
  }

}
