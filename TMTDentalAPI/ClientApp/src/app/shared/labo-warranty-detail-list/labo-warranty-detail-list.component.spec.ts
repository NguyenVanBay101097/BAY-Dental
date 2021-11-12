import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboWarrantyDetailListComponent } from './labo-warranty-detail-list.component';

describe('LaboWarrantyDetailListComponent', () => {
  let component: LaboWarrantyDetailListComponent;
  let fixture: ComponentFixture<LaboWarrantyDetailListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboWarrantyDetailListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboWarrantyDetailListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
