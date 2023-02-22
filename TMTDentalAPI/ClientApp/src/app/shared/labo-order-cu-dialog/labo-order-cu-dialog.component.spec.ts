import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderCuDialogComponent } from './labo-order-cu-dialog.component';

describe('LaboOrderCuDialogComponent', () => {
  let component: LaboOrderCuDialogComponent;
  let fixture: ComponentFixture<LaboOrderCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
