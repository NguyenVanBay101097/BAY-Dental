import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderCuLineDialogComponent } from './labo-order-cu-line-dialog.component';

describe('LaboOrderCuLineDialogComponent', () => {
  let component: LaboOrderCuLineDialogComponent;
  let fixture: ComponentFixture<LaboOrderCuLineDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderCuLineDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderCuLineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
