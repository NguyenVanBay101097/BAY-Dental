import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedCardCardGridComponent } from './shared-card-card-grid.component';

describe('SharedCardCardGridComponent', () => {
  let component: SharedCardCardGridComponent;
  let fixture: ComponentFixture<SharedCardCardGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SharedCardCardGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedCardCardGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
