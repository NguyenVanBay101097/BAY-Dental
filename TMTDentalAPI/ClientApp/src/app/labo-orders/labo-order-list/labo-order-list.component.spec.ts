import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderListComponent } from './labo-order-list.component';

describe('LaboOrderListComponent', () => {
  let component: LaboOrderListComponent;
  let fixture: ComponentFixture<LaboOrderListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
