import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UomImportExcelDialogComponent } from './uom-import-excel-dialog.component';

describe('UomImportExcelDialogComponent', () => {
  let component: UomImportExcelDialogComponent;
  let fixture: ComponentFixture<UomImportExcelDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UomImportExcelDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UomImportExcelDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
