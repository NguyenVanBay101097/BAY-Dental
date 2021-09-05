import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentFilterExportExcelDialogComponent } from './appointment-filter-export-excel-dialog.component';

describe('AppointmentFilterExportExcelDialogComponent', () => {
  let component: AppointmentFilterExportExcelDialogComponent;
  let fixture: ComponentFixture<AppointmentFilterExportExcelDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentFilterExportExcelDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentFilterExportExcelDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
