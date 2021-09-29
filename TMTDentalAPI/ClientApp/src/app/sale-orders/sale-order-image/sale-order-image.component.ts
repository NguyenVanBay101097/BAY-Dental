import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { WebService } from 'src/app/core/services/web.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { IrAttachmentService } from 'src/app/shared/ir-attachment.service';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-sale-order-image',
  templateUrl: './sale-order-image.component.html',
  styleUrls: ['./sale-order-image.component.css']
})
export class SaleOrderImageComponent implements OnInit {
  @Input('saleOrderId') saleOrderId: string;
  imagesPreview: any[] = []
  dataFilter: any;

  constructor(
    private modalService: NgbModal,
    public notifyService: NotifyService,
    private saleOrderService: SaleOrderService,
    private webService: WebService,
    private irAttachmentService: IrAttachmentService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  convertData() {
    let data = [];
    let arr = [...this.imagesPreview];

    arr.sort(function (a, b) {
      return (new Date(b.dateCreated)).valueOf() - (new Date(a.dateCreated)).valueOf();
    });

    data = arr.reduce((r, a) => {
      const dateFormat = moment(a.dateCreated).format('YYYY-MM-DD');
      r[dateFormat] = r[dateFormat] || [];
      r[dateFormat].push(a);
      return r;
    }, Object.create(null));

    this.dataFilter = Object.keys(data).map((key) => [key, data[key]]);
  }

  loadData() {
    this.saleOrderService.getListAttachment(this.saleOrderId).subscribe((res: any) => {
      this.imagesPreview = res
      this.convertData();
    }, err => console.log(err))
  }

  addImages(e) {
    var file_node = e.target;
    var count = file_node.files.length;
    var formData = new FormData();
    formData.append('id', this.saleOrderId);
    formData.append('model', 'sale.order');

    for (let i = 0; i < count; i++) {
      var file = file_node.files[i];
      let size = file.size / (1024 * 1024);
      if (size > 30) {
        this.notifyService.notify('error', 'Không thể thêm ảnh có dung lượng lớn hơn 30MB');
        return false;
      }

      formData.append('files', file);
    }
    this.webService.binaryUploadAttachment(formData).subscribe((res: any) => {
      this.loadData();
    }, (err) => { console.log(err) })
  }

  viewImage(image) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.images = this.imagesPreview;
    modalRef.componentInstance.selectedImage = image;
  }

  deleteImages(item, event) {
    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa hình ảnh';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa hình ảnh?';
    modalRef.result.then(() => {
      this.irAttachmentService.deleteImage(item.id).subscribe((res: any) => {
        let index = this.imagesPreview.findIndex(x => x.id === item.id);
        if (index != -1) {
          this.imagesPreview.splice(index, 1);
          this.convertData();
          this.notifyService.notify('success', 'Xóa thành công');
        }
      })
    })
  }

  getTitleDate(dateStr) {
    const yesterday = moment().subtract(1, 'days').format('YYYY-MM-DD');
    const today = moment().format('YYYY-MM-DD');
    switch (dateStr) {
      case yesterday:
        return 'Hôm qua';
      case today:
        return 'Hôm nay';
      default:
        return moment(new Date(dateStr)).locale('vi').format('LL');
    }
  }

  stopPropagation(e) {
    e.stopPropagation();
  }

}
