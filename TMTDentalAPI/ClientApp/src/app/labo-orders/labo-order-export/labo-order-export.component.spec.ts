import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderExportComponent } from './labo-order-export.component';

describe('LaboOrderExportComponent', () => {
  let component: LaboOrderExportComponent;
  let fixture: ComponentFixture<LaboOrderExportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderExportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderExportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
