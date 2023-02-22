import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderListDialogComponent } from './labo-order-list-dialog.component';

describe('LaboOrderListDialogComponent', () => {
  let component: LaboOrderListDialogComponent;
  let fixture: ComponentFixture<LaboOrderListDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderListDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderListDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
