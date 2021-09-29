import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
@Component({
  selector: 'app-sale-order-images-library-popup',
  templateUrl: './sale-order-images-library-popup.component.html',
  styleUrls: ['./sale-order-images-library-popup.component.css']
})
export class SaleOrderImagesLibraryPopupComponent implements OnInit {

  images: any = [];
  imagesGroup: any[] = [];
  imagesSelected: any[] = [];
  search: string ='';
  searchUpdate = new Subject<string>();
  data: any[] =[];
  items: any[] = [];
  id: string = '';
  constructor(public activeModal: NgbActiveModal,private intlService: IntlService,
    private saleOrderService: SaleOrderService
    ) { }
  
  ngOnInit() {
    this.images = [
      // {name:'Hình 1',id:'1',uploadId:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', dateCreated: "2021-09-28T10:52:39"},
      // {name:'Hình 2',id:'2',uploadId:'https://image.thanhnien.vn/1024x768/uploaded/chicuong/2021_06_11/chiron/03-02-bugatti-chiron-super-sport-molsheim-3-4-front-hr_cqan.jpeg', dateCreated: "2021-09-28T10:52:39"},
      // {name:'Hình 3',id:'3',uploadId:'https://giaxenhap.com/wp-content/uploads/2021/07/thumb-bugatti-chiron.jpg', dateCreated: "2021-09-27T10:52:39"},
      // {name:'Hình 4',id:'4',uploadId:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', dateCreated: "2021-09-25T10:52:39"},
      // {name:'Hình 5',id:'5',uploadId:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', dateCreated: "2021-09-26T10:52:39"},
      // {name:'Hình 6',id:'6',uploadId:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', dateCreated: "2021-09-28T10:52:39"},
      // {name:'Hình 7',id:'7',uploadId:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', dateCreated: "2021-09-27T10:52:39"},
      // {name:'Hình 8',id:'8',uploadId:'https://image.thanhnien.vn/1024x768/uploaded/chicuong/2021_06_11/chiron/03-02-bugatti-chiron-super-sport-molsheim-3-4-front-hr_cqan.jpeg',dateCreated: "2021-09-28T10:52:39"},
      // {name:'Hình 9',id:'9',uploadId:'https://giaxenhap.com/wp-content/uploads/2021/07/thumb-bugatti-chiron.jpg', dateCreated: "2021-09-28T10:52:39"},
      // {name:'Hình 10',id:'10', uploadId:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', dateCreated: "2021-09-27T10:52:39"},
      // {name:'Hình 11',id:'11', uploadId:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', dateCreated: "2021-09-25T10:52:39"},
      // {name:'Hình 12',id:'12', uploadId:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', dateCreated: "2021-09-26T10:52:39"}
    ];
    this.loadImages();
    console.log(this.imagesGroup);
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((val) => {
        var items = this.images.filter((img,index)=> {
          return img.name.toUpperCase().indexOf(val) != -1
        });
        this.groupByImages(items);
      });
  }

  loadImages(){
    this.saleOrderService.getListAttachment(this.id).subscribe(res => {
      console.log(res);
      
      this.images = res;
      this.groupByImages(res);
    })
  }

  onChoose(){
    this.activeModal.close(this.imagesSelected);
  }

  onCancel(){
    this.activeModal.close([]);
  }

  chooseImage(img){
    if (this.imagesSelected.length == 0){
      this.imagesSelected.push(img);
      return;
    }
      
    var index = this.imagesSelected.findIndex(x => x.id == img.id);
    if (index != -1)
      this.imagesSelected.splice(index,1);
    else
      this.imagesSelected.push(img);
  }

  isSelected(img){
    var item = this.imagesSelected.find(x => x.id == img.id);
    if (item)
      return true;
    return false;
  }

  groupByImages(items:any){
    let data = null;
    data = items.reduce((r, a) => {
      const dateFormat = moment(a.dateCreated).format('DD/MM/YYYY');
      r[dateFormat] = r[dateFormat] || [];
      r[dateFormat].push(a);
      return r;
    }, Object.create(null));

    this.imagesGroup = Object.keys(data).map( (key)=> [key, data[key]]);
  }

  getTitleDate(dateStr){
    let dateObj = dateStr;
    let currentDate = moment(Date.now()).format('DD/MM/YYYY');
    let preDate = moment().subtract(1, 'days').format('DD/MM/YYYY');
    if (dateObj == currentDate)
      return 'Hôm nay';
    else if (dateObj == preDate)
      return 'Hôm qua';
    else 
      return dateObj;
  }
}
