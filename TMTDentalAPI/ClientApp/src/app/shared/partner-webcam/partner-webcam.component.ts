import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { WebService } from 'src/app/core/services/web.service';
declare var Webcam: any;
@Component({
  selector: 'app-partner-webcam',
  templateUrl: './partner-webcam.component.html',
  styleUrls: ['./partner-webcam.component.css']
})
export class PartnerWebcamComponent implements OnInit {

  image64: any;

  constructor(private activeModal: NgbActiveModal, private webService: WebService) { }

  ngOnInit() {
  }

  public ngAfterViewInit() {
    Webcam.set({
      width: 320,
      height: 240,
      image_format: 'jpeg',
      jpeg_quality: 90
    });
    Webcam.attach('#my_camera');
  }

  onSnap() {
    Webcam.snap(function (data_uri) {
      this.image64 = data_uri;
      document.getElementById('results').innerHTML =
      '<a href="'+ this.image64 +'" download="cbimage.jpg"><img style="width: 100%; height: 100%;" src="' + this.image64 + '"/></a>';
    });
  }

  onSave() {
    if (!this.image64) {
      this.activeModal.close();
      return;
    }
    // convert base 64 to file
    var file = this.dataURLtoFile(this.image64, 'abc.png');
    this.activeModal.close(file);
  }

  dataURLtoFile(dataurl, filename) {

    var arr = dataurl.split(','),
      mime = arr[0].match(/:(.*?);/)[1],
      bstr = atob(arr[1]),
      n = bstr.length,
      u8arr = new Uint8Array(n);

    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }

    return new File([u8arr], filename, { type: mime });
  }

}
