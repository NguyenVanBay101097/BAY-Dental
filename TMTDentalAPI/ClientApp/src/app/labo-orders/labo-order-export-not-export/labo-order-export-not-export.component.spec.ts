import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderExportNotExportComponent } from './labo-order-export-not-export.component';

describe('LaboOrderExportNotExportComponent', () => {
  let component: LaboOrderExportNotExportComponent;
  let fixture: ComponentFixture<LaboOrderExportNotExportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderExportNotExportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderExportNotExportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
