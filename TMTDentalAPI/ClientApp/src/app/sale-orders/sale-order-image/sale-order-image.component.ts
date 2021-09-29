import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-sale-order-image',
  templateUrl: './sale-order-image.component.html',
  styleUrls: ['./sale-order-image.component.css']
})
export class SaleOrderImageComponent implements OnInit {
  imagesPreview = [
    {
      id: '1',
      name: 'ảnh số 1',
      date: '2021-09-28',
      note: 'ghi chú 1',
      uploadId: 'https://cdn.pixabay.com/photo/2021/09/06/01/13/coffee-6600644__340.jpg'
    },
    {
      id: '2',
      name: 'ảnh số 2',
      date: '2021-09-23',
      note: 'ghi chú 2',
      uploadId: 'https://cdn.pixabay.com/photo/2020/11/25/03/04/russian-blue-cat-5774414__340.jpg'
    },
    {
      id: '3',
      name: 'ảnh số 3',
      date: '2021-10-15',
      note: 'ghi chú 3',
      uploadId: 'https://cdn.pixabay.com/photo/2021/09/06/08/59/tower-6601206__340.jpg'
    },
    {
      id: '4',
      name: 'ảnh số 4',
      date: '2021-09-23',
      note: 'ghi chú 4',
      uploadId: 'https://cdn.pixabay.com/photo/2021/09/17/12/14/coffee-6632533__340.jpg'
    },
    {
      id: '5',
      name: 'ảnh số 5',
      date: '2021-09-23',
      note: 'ghi chú 3',
      uploadId: 'https://cdn.pixabay.com/photo/2021/09/06/08/59/tower-6601206__340.jpg'
    },
    {
      id: '6',
      name: 'ảnh số 6',
      date: '2021-09-23',
      note: 'ghi chú 4',
      uploadId: 'https://cdn.pixabay.com/photo/2021/09/17/12/14/coffee-6632533__340.jpg'
    },
  ]
  @Input('saleOrderId') saleOrderId: string;
  dataFilter: any
  constructor(
    private modalService: NgbModal,
    public notifyService: NotifyService

  ) { }

  ngOnInit() {
    this.loadData();
  }

  convertData() {
    let data = null;
    let arr = [...this.imagesPreview];

    arr.sort(function (a, b) {
      return (new Date(a.date)).valueOf() - (new Date(b.date)).valueOf();
    });

    data = arr.reduce((r, a) => {
      const dateFormat = moment(a.date).format('DD/MM/YYYY');
      r[dateFormat] = r[dateFormat] || [];
      r[dateFormat].push(a);
      return r;
    }, Object.create(null));

    this.dataFilter = Object.keys(data).map((key) => [key, data[key]]);

  }

  loadData() {
    // call api

    this.convertData();
  }

  addImages(e) {
    // var file_node = e.target;
    // var count = file_node.files.length;
    // var formData = new FormData();
    // for (let i = 0; i < count; i++) {
    //   var file = file_node.files[i];
    //   console.log(file);
    //   console.log(file.size);

    //   let size = file.size / (1024 * 1024);
    //   if (size > 30) {
    //     this.notifyService.notify('error', 'Không thể thêm ảnh có dung lượng lớn hơn 30MB');
    //     return false;
    //   }

    //   formData.append('files', file);
    //   var filereader = new FileReader();
    //   filereader.readAsDataURL(file);
    // }

    const a = {
      id: '4',
      name: 'ảnh số 4',
      date: '2021-09-23',
      note: 'ghi chú 4',
      uploadId: 'https://cdn.pixabay.com/photo/2021/09/17/12/14/coffee-6632533__340.jpg'
    }
    this.imagesPreview.push(a);
    this.convertData();
  }

  viewImage(image) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.partnerImages = this.imagesPreview;
    modalRef.componentInstance.partnerImageSelected = image;
  }

  deleteImages(item, event) {
    // var index = this.imagesPreview.find(item.id);
    // console.log(index);

    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa hình ảnh';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa hình ảnh';
    modalRef.result.then(() => {
      // this.partnerService.deleteParnerImage(item.id).subscribe(
      //   () => {
      //     this.convertData();
      //   })
      this.notifyService.notify('success', 'Xóa thành công');
    })
  }

  stopPropagation(e) {
    e.stopPropagation();
  }

}
