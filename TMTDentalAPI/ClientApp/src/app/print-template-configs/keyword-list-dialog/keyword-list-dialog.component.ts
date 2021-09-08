import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-keyword-list-dialog',
  templateUrl: './keyword-list-dialog.component.html',
  styleUrls: ['./keyword-list-dialog.component.css']
})
export class KeywordListDialogComponent implements OnInit {
  search = '';
  searchUpdate = new Subject<string>();
  @Input() boxKeyWordSource = [];
  @Input() boxKeyWordList = [];
  constructor(
    public activeModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.boxKeyWordList = JSON.parse(JSON.stringify(this.boxKeyWordSource));
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        var temp = JSON.parse(JSON.stringify(this.boxKeyWordSource))
        temp = temp.filter(x => {
          x.value = x.value.slice().filter(z => z.text.toLowerCase().includes(value.toLowerCase()))
          return x.value.length > 0;
        });
        this.boxKeyWordList = temp;
      });
  }

  onSelectKeyWord(item) {
    this.activeModal.close(item);
  }

}
