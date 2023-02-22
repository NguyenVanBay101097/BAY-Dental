import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderExportDialogComponent } from './labo-order-export-dialog.component';

describe('LaboOrderExportDialogComponent', () => {
  let component: LaboOrderExportDialogComponent;
  let fixture: ComponentFixture<LaboOrderExportDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderExportDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderExportDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
