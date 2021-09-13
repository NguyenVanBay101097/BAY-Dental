import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderWarrantyListComponent } from './labo-order-warranty-list.component';

describe('LaboOrderWarrantyListComponent', () => {
  let component: LaboOrderWarrantyListComponent;
  let fixture: ComponentFixture<LaboOrderWarrantyListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderWarrantyListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderWarrantyListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
