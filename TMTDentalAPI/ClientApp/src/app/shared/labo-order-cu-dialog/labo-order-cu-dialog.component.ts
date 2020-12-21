import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LaboImageBasic } from 'src/app/labo-orders/labo-order.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from '../image-viewer/image-viewer.component';

@Component({
  selector: 'app-labo-order-cu-dialog',
  templateUrl: './labo-order-cu-dialog.component.html',
  styleUrls: ['./labo-order-cu-dialog.component.css']
})
export class LaboOrderCuDialogComponent implements OnInit {
  title: string;
  myForm: FormGroup;

  NCCLaboList: any = [];

  imagesPreview: LaboImageBasic[] = [];

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private modalService: NgbModal) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      NCCLabo: [null, Validators.required], 
      dateObj: null, 
    });

    setTimeout(() => {
      this.loadNCCLaboList();

    });
  }

  loadNCCLaboList() {

  }

  stopPropagation(e) {
    e.stopPropagation();
  }

  addImages(e) {
    var file_node = e.target;
    var count = file_node.files.length;
    // var formData = new FormData();
    // formData.append('partnerId', this.partnerId);
    for (let i = 0; i < count; i++) {
      var file = file_node.files[i];
      // formData.append('files', file);
      var filereader = new FileReader();
      filereader.readAsDataURL(file);
    }
    // this.partnerService.uploadPartnerImage(formData).subscribe(() => {
    //   this.getImageIds();
    // }, (err) => {
    //   this.showErrorService.show(err);
    // });
  }

  deleteImages(index, event) {
    var item = this.imagesPreview[index];
    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa hình ảnh ' + item.name;

    modalRef.result.then(() => {
      // this.partnerService.deleteParnerImage(item.id).subscribe(
      //   () => {
      //     this.loadData();
      //   })
    })
  }

  viewImage(laboImage: LaboImageBasic) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.partnerImages = this.imagesPreview;
    modalRef.componentInstance.partnerImageSelected = laboImage;
  }

  onSave() {

  }
}
