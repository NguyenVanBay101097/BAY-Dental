import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotKhamDisplay } from 'src/app/dot-khams/dot-khams';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { PartnerImageBasic } from 'src/app/shared/services/partners.service';

@Component({
  selector: 'app-partner-dotkham-detail',
  templateUrl: './partner-dotkham-detail.component.html',
  styleUrls: ['./partner-dotkham-detail.component.css']
})
export class PartnerDotkhamDetailComponent implements OnInit {

  constructor(
    public dotkhamService: DotKhamService,
    private modalService: NgbModal
  ) { }

  @Input() dotkhamId: string;
  dotkham: DotKhamDisplay = new DotKhamDisplay();

  ngOnInit() {

    this.loadDotkham();
  }

  loadDotkham() {
    this.dotkhamService.get(this.dotkhamId).subscribe(
      res => {
        this.dotkham = res;
      }
    );
  }

  showLineTeeth(teeth) {
    return teeth.map(x=> x.name).join(',');
  }

  stopPropagation(e)
  {
    return e.stopPropagation();
  }


  onViewImg(img: any) {
    const modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
   
    modalRef.componentInstance.partnerImages = this.dotkham.dotKhamImages;
    modalRef.componentInstance.partnerImageSelected = img;
  }
}
