import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HistoryImportExcelDialogComponent } from './history-import-excel-dialog.component';

describe('HistoryImportExcelDialogComponent', () => {
  let component: HistoryImportExcelDialogComponent;
  let fixture: ComponentFixture<HistoryImportExcelDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HistoryImportExcelDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HistoryImportExcelDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
