import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderDetailListComponent } from './labo-order-detail-list.component';

describe('LaboOrderDetailListComponent', () => {
  let component: LaboOrderDetailListComponent;
  let fixture: ComponentFixture<LaboOrderDetailListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderDetailListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderDetailListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
