import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
@Component({
  selector: 'app-sale-order-images-library-popup',
  templateUrl: './sale-order-images-library-popup.component.html',
  styleUrls: ['./sale-order-images-library-popup.component.css']
})
export class SaleOrderImagesLibraryPopupComponent implements OnInit {

  images: any[] = [];
  imagesGroup: any[] = [];
  imagesSelected: any[] = [];
  search: string ='';
  searchUpdate = new Subject<string>();
  data: any[] =[];
  items: any[] = [];
  constructor(public activeModal: NgbActiveModal,private intlService: IntlService) { }
  
  ngOnInit() {
    this.images = [
      {name:'Hình 1', url:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', date: "2021-09-28T10:52:39"},
      {name:'Hình 2', url:'https://image.thanhnien.vn/1024x768/uploaded/chicuong/2021_06_11/chiron/03-02-bugatti-chiron-super-sport-molsheim-3-4-front-hr_cqan.jpeg', date: "2021-09-28T10:52:39"},
      {name:'Hình 3', url:'https://giaxenhap.com/wp-content/uploads/2021/07/thumb-bugatti-chiron.jpg', date: "2021-09-27T10:52:39"},
      {name:'Hình 4', url:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', date: "2021-09-25T10:52:39"},
      {name:'Hình 5', url:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', date: "2021-09-26T10:52:39"},
      {name:'Hình 6', url:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', date: "2021-09-28T10:52:39"},
      {name:'Hình 7', url:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', date: "2021-09-27T10:52:39"},
      {name:'Hình 8', url:'https://image.thanhnien.vn/1024x768/uploaded/chicuong/2021_06_11/chiron/03-02-bugatti-chiron-super-sport-molsheim-3-4-front-hr_cqan.jpeg',date: "2021-09-28T10:52:39"},
      {name:'Hình 9', url:'https://giaxenhap.com/wp-content/uploads/2021/07/thumb-bugatti-chiron.jpg', date: "2021-09-28T10:52:39"},
      {name:'Hình 10', url:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', date: "2021-09-27T10:52:39"},
      {name:'Hình 11', url:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', date: "2021-09-25T10:52:39"},
      {name:'Hình 12', url:'https://pe-images.s3.amazonaws.com/basics/cc/image-size-resolution/resize-images-for-print/image-cropped-8x10.jpg', date: "2021-09-26T10:52:39"}
    ];

    this.groupByImages(this.images);
    console.log(this.imagesGroup);
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((val) => {
        var items = this.images.filter((img,index)=> {
          return img.name.toUpperCase().indexOf(val) != -1;
        });
        this.groupByImages(items);
      });
  }

  onChoose(){
    this.activeModal.close(this.imagesSelected);
  }

  onCancel(){
    this.activeModal.close();
  }

  chooseImage(img){
    if (this.imagesSelected.length == 0){
      this.imagesSelected.push(img);
      return;
    }
      
    var index = this.imagesSelected.findIndex(x => x.name == img.name);
    if (index != -1)
      this.imagesSelected.splice(index,1);
    else
      this.imagesSelected.push(img);
  }

  isSelected(img){
    var item = this.imagesSelected.find(x => x.name == img.name);
    if (item)
      return true;
    return false;
  }

  groupByImages(items:any[]){
    let data = null;
    data = items.reduce((r, a) => {
      const dateFormat = moment(a.date).format('DD/MM/YYYY');
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
