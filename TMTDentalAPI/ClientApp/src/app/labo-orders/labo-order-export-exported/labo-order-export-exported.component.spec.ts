import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderExportExportedComponent } from './labo-order-export-exported.component';

describe('LaboOrderExportExportedComponent', () => {
  let component: LaboOrderExportExportedComponent;
  let fixture: ComponentFixture<LaboOrderExportExportedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderExportExportedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderExportExportedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
