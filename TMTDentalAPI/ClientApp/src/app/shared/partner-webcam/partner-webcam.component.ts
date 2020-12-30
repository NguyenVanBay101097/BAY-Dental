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
      height: 240,
      image_format: 'jpeg',
      jpeg_quality: 90
    });
    Webcam.attach('#my_camera');
    var videoEls = document.getElementById('my_camera').getElementsByTagName("video");
    if(videoEls.length > 0) {
      videoEls[0].style.width = '100%';
    }
  }

  onSnap() {
    var self = this;
    Webcam.snap(function (data_uri) {
      self.image64 = data_uri;
      document.getElementById('results').innerHTML =
        '<a href="' + self.image64 + '" download="cbimage.jpg"><img style="width: 100%; height: 100%;" src="' + self.image64 + '"/></a>';
    });
  }

  onSave() {
    if (!this.image64) {
      this.activeModal.close();
      return;
    }
    this.webService.UploadImageByBase64(this.image64).subscribe((result: any) => {
      Webcam.reset();
      this.activeModal.close(result);
    });
  }

  onClose() {
    Webcam.reset();
    this.activeModal.close();
  }

}
